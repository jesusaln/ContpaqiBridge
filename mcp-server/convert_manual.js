#!/usr/bin/env node
/**
 * Convierte las páginas HTML del manual SDK de CONTPAQi a Markdown limpio.
 *
 * Estrategia:
 *   - Extrae el <article>...</article>
 *   - Convierte headings h1-h5 a #..#####
 *   - Convierte tablas a markdown
 *   - Convierte listas <ul>/<ol> (recursivas) a markdown
 *   - Limpia divs vacíos, marcadores sueltos, imágenes decorativas
 *   - Decodifica entidades HTML
 *
 * Uso: node convert_manual.js
 */
'use strict';

const fs = require('fs');
const path = require('path');
const https = require('https');
const { URL } = require('url');

const MANUAL_BASE_URL = 'https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK';
const SRC_DIR = path.join(__dirname, '..', 'sdk_pages');
const OUT_DIR = path.join(__dirname, 'manual_md');

// ====================================================================
// DESCARGA DE PÁGINAS HTML
// ====================================================================

function downloadPage(url, outPath) {
    return new Promise((resolve, reject) => {
        const u = new URL(url);
        const req = https.get(u, { timeout: 30000 }, (res) => {
            if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
                return downloadPage(res.headers.location, outPath).then(resolve, reject);
            }
            if (res.statusCode !== 200) {
                reject(new Error(`HTTP ${res.statusCode} en ${url}`));
                return;
            }
            const chunks = [];
            res.on('data', (c) => chunks.push(c));
            res.on('end', () => {
                fs.writeFileSync(outPath, Buffer.concat(chunks));
                resolve(outPath);
            });
            res.on('error', reject);
        });
        req.on('error', reject);
        req.on('timeout', () => req.destroy(new Error('timeout')));
    });
}

function discoverPages() {
    // El manual estático de CONTPAQi usa un índice JS llamado "all.js" que
    // contiene el array DREX_NODE_LINKS con todas las URLs.
    return new Promise((resolve, reject) => {
        const u = new URL(MANUAL_BASE_URL + '/js/all.js');
        https.get(u, { timeout: 30000 }, (res) => {
            if (res.statusCode !== 200) {
                reject(new Error(`HTTP ${res.statusCode} al obtener índice`));
                return;
            }
            let body = '';
            res.setEncoding('utf8');
            res.on('data', (c) => body += c);
            res.on('end', () => {
                const m = body.match(/DREX_NODE_LINKS:\s*\[([^\]]+)\]/);
                if (!m) {
                    reject(new Error('No se encontró DREX_NODE_LINKS en all.js'));
                    return;
                }
                const links = Array.from(m[1].matchAll(/"([^"]+)"/g), (x) => x[1]);
                resolve(links);
            });
            res.on('error', reject);
        }).on('error', reject);
    });
}

async function downloadAll() {
    if (!fs.existsSync(SRC_DIR)) fs.mkdirSync(SRC_DIR, { recursive: true });
    console.log('Descubriendo páginas del manual...');
    const pages = await discoverPages();
    console.log(`Encontradas ${pages.length} páginas. Descargando en paralelo...`);

    const results = await Promise.allSettled(pages.map(async (page) => {
        const url = MANUAL_BASE_URL + '/' + page;
        const out = path.join(SRC_DIR, page);
        try {
            await downloadPage(url, out);
            return { ok: true, page };
        } catch (err) {
            return { ok: false, page, error: err.message };
        }
    }));

    let ok = 0, fail = 0;
    for (const r of results) {
        if (r.value.ok) ok++;
        else {
            fail++;
            console.warn(`  [fail] ${r.value.page}: ${r.value.error}`);
        }
    }
    console.log(`Descarga completa: ${ok} OK, ${fail} fallidas.`);
}

// ====================================================================
// CONVERSIÓN HTML -> MARKDOWN
// ====================================================================

// ====================================================================
// UTILIDADES
// ====================================================================

function decodeEntities(s) {
    if (!s) return '';
    return s
        .replace(/&#160;/g, ' ')
        .replace(/&#xa0;/g, ' ')
        .replace(/&nbsp;/g, ' ')
        .replace(/&amp;/g, '&')
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/&quot;/g, '"')
        .replace(/&apos;/g, "'")
        .replace(/&#39;/g, "'")
        .replace(/&#(\d+);/g, (_, n) => String.fromCharCode(parseInt(n, 10)))
        .replace(/&#x([0-9a-fA-F]+);/g, (_, n) => String.fromCharCode(parseInt(n, 16)));
}

function readFile(filePath) {
    const buf = fs.readFileSync(filePath);
    let s = buf.toString('utf8');
    if (buf[0] === 0xEF && buf[1] === 0xBB && buf[2] === 0xBF) {
        s = buf.slice(3).toString('utf8');
    }
    return s;
}

// Quita todos los tags HTML pero conserva texto y entidades. Opcionalmente añade \n en <br>, <p>, <div>.
function stripTags(html, blockTags = new Set(['p', 'div', 'li', 'tr', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6'])) {
    let out = html;
    out = out.replace(/<br\s*\/?>/gi, '\n');
    out = out.replace(/<\/(p|div|li|tr|h[1-6]|ul|ol|table|pre)>/gi, '\n');
    out = out.replace(/<[^>]+>/g, '');
    out = decodeEntities(out);
    // Colapsar espacios múltiples
    out = out.replace(/[ \t]+/g, ' ');
    out = out.replace(/ ?\n ?/g, '\n');
    return out;
}

// ====================================================================
// EXTRACTORES DE BLOQUES
// ====================================================================

function extractArticle(html) {
    const m = html.match(/<article>([\s\S]*?)<\/article>/i);
    return m ? m[1] : '';
}

// Tabla -> markdown
function convertTable(tableHtml) {
    const rows = [];
    const trMatches = tableHtml.match(/<tr[\s\S]*?<\/tr>/gi) || [];
    for (const tr of trMatches) {
        const cells = [];
        const cellMatches = tr.match(/<t[hd][^>]*>[\s\S]*?<\/t[hd]>/gi) || [];
        for (const cell of cellMatches) {
            // Procesar el contenido: <p>, <br>, strong, etc.
            let c = cell;
            c = c.replace(/<br\s*\/?>/gi, '<brNL>');
            c = c.replace(/<\/(p|div)>/gi, '<brNL>');
            c = c.replace(/<(strong|b)[^>]*>([\s\S]*?)<\/\1>/gi, '$2');
            c = c.replace(/<[^>]+>/g, '');
            c = decodeEntities(c).replace(/<brNL>/g, ' ').replace(/\s+/g, ' ').trim();
            cells.push(c);
        }
        if (cells.length) rows.push(cells);
    }
    if (!rows.length) return '';

    const cols = Math.max(...rows.map(r => r.length));
    const norm = rows.map(r => {
        while (r.length < cols) r.push('');
        return r.map(c => c.replace(/\|/g, '\\|'));
    });

    let md = '';
    md += '| ' + norm[0].join(' | ') + ' |\n';
    md += '|' + norm[0].map(() => '---').join('|') + '|\n';
    for (let i = 1; i < norm.length; i++) {
        md += '| ' + norm[i].join(' | ') + ' |\n';
    }
    return md;
}

// Lista <ul>/<ol> recursiva. Devuelve un string con indentación.
function convertList(html, depth = 0, ordered = false, startIndex = 0) {
    const indent = '  '.repeat(depth);
    const items = [];
    // Capturar <li>...</li> (puede haber anidados)
    let i = 0;
    let pos = 0;
    const re = /<li[^>]*>([\s\S]*?)(?=<li|<\/(ul|ol)>|$)/gi;
    let m;
    while ((m = re.exec(html)) !== null) {
        let itemContent = m[1];
        // Limpiar: extraer ul/ol anidados primero
        const innerLists = [];
        itemContent = itemContent.replace(/<(ul|ol)[^>]*>([\s\S]*?)<\/\1>/gi, (_, tag, inner) => {
            innerLists.push({ tag, html: inner });
            return `\n<<INNERLIST_${innerLists.length - 1}>>`;
        });

        // Quitar tags, limpiar texto
        let text = stripTags(itemContent);
        text = text.replace(/\n+/g, ' ').replace(/\s+/g, ' ').trim();

        if (!text && innerLists.length === 0) continue;

        const marker = ordered ? `${startIndex + i + 1}.` : '-';
        let md = `${indent}${marker} ${text}`;
        if (innerLists.length) {
            md += '\n';
            for (let k = 0; k < innerLists.length; k++) {
                md += convertList(innerLists[k].html, depth + 1, innerLists[k].tag === 'ol', 0) + '\n';
            }
        }
        items.push(md);
        i++;
        pos = m.index + m[0].length;
    }
    return items.join('\n');
}

// ====================================================================
// CONVERTIDOR PRINCIPAL
// ====================================================================

function htmlToMarkdown(articleHtml) {
    let html = articleHtml;

    // Eliminar divs sociales y de tracking
    html = html.replace(/<div class="b-socialplugin[\s\S]*?<\/div>\s*<\/div>/g, '');
    html = html.replace(/<img[^>]*counter[\s\S]*?>/g, '');
    html = html.replace(/<div class="fb-like"[\s\S]*?<\/div>/g, '');

    // Eliminar todas las imágenes decorativas (logos, diagramas sin alt útil)
    html = html.replace(/<img[^>]*>/g, '');

    // Eliminar divs vacíos o con solo &#160;
    html = html.replace(/<div[^>]*>\s*(?:&#160;|&nbsp;|\s)*<\/div>/gi, '');

    // Extraer tablas
    const tables = [];
    html = html.replace(/<table[\s\S]*?<\/table>/gi, (m) => {
        const md = convertTable(m);
        tables.push(md);
        return `\n\n##TABLE##${tables.length - 1}##\n\n`;
    });

    // Extraer listas
    const lists = [];
    html = html.replace(/<(ul|ol)[^>]*>([\s\S]*?)<\/\1>/gi, (_, tag, inner) => {
        const md = convertList(inner, 0, tag === 'ol');
        if (md) {
            lists.push(md);
            return `\n\n##LIST##${lists.length - 1}##\n\n`;
        }
        return '';
    });

    // Extraer bloques <pre> (código)
    const pres = [];
    html = html.replace(/<pre[\s\S]*?<\/pre>/gi, (m) => {
        let inner = m.replace(/<[^>]+>/g, '');
        inner = decodeEntities(inner);
        pres.push('```\n' + inner.trim() + '\n```');
        return `\n\n##PRE##${pres.length - 1}##\n\n`;
    });

    // Headings
    html = html.replace(/<h1[^>]*>([\s\S]*?)<\/h1>/gi, (_, c) => `\n\n# ${stripTagsInline(c)}\n\n`);
    html = html.replace(/<h2[^>]*>([\s\S]*?)<\/h2>/gi, (_, c) => `\n\n## ${stripTagsInline(c)}\n\n`);
    html = html.replace(/<h3[^>]*>([\s\S]*?)<\/h3>/gi, (_, c) => `\n\n### ${stripTagsInline(c)}\n\n`);
    html = html.replace(/<h4[^>]*>([\s\S]*?)<\/h4>/gi, (_, c) => `\n\n#### ${stripTagsInline(c)}\n\n`);
    html = html.replace(/<h5[^>]*>([\s\S]*?)<\/h5>/gi, (_, c) => `\n\n##### ${stripTagsInline(c)}\n\n`);

    // Formato inline
    html = html.replace(/<(strong|b)[^>]*>([\s\S]*?)<\/\1>/gi, (_, _t, c) => `**${stripTagsInline(c)}**`);
    html = html.replace(/<(em|i)[^>]*>([\s\S]*?)<\/\1>/gi, (_, _t, c) => `*${stripTagsInline(c)}*`);
    html = html.replace(/<code[^>]*>([\s\S]*?)<\/code>/gi, (_, c) => `\`${stripTagsInline(c)}\``);

    // Links
    html = html.replace(/<a[^>]*href="([^"]+)"[^>]*>([\s\S]*?)<\/a>/gi, (_, href, c) => {
        const txt = stripTagsInline(c);
        if (!txt || txt === href) return href;
        return `[${txt}](${href})`;
    });

    // Párrafos y divs con texto
    html = html.replace(/<div class="p[^"]*"[^>]*>([\s\S]*?)<\/div>/gi, (_, c) => {
        const text = stripTagsInline(c);
        if (!text) return '';
        return `\n${text}\n`;
    });

    // Cualquier <div> restante -> salto
    html = html.replace(/<div[^>]*>/gi, '\n');
    html = html.replace(/<\/div>/gi, '\n');

    // Quitar tags restantes
    html = html.replace(/<[^>]+>/g, '');

    // Decodificar entidades
    html = decodeEntities(html);

    // Restaurar tablas/listas
    html = html.replace(/##TABLE##(\d+)##/g, (_, i) => tables[parseInt(i, 10)]);
    html = html.replace(/##LIST##(\d+)##/g, (_, i) => lists[parseInt(i, 10)]);
    html = html.replace(/##PRE##(\d+)##/g, (_, i) => pres[parseInt(i, 10)]);

    // Limpiar líneas vacías múltiples
    html = html.replace(/\n{3,}/g, '\n\n');
    html = html.replace(/[ \t]+\n/g, '\n');
    // Quitar líneas que solo tengan marcadores sueltos como ">>", "**", etc.
    html = html.replace(/^\s*(>>|<<|<\?|!)\s*$/gm, '');
    html = html.replace(/^\s*$/gm, '');
    html = html.replace(/\n{3,}/g, '\n\n');
    html = html.replace(/^\s+/, '').replace(/\s+$/, '');

    return html;
}

function stripTagsInline(s) {
    if (!s) return '';
    let t = s.replace(/<br\s*\/?>/gi, ' ');
    t = t.replace(/<[^>]+>/g, '');
    t = decodeEntities(t);
    t = t.replace(/\s+/g, ' ').trim();
    return t;
}

function extractTitle(html) {
    const article = extractArticle(html);
    // Buscar h1, sino h2, sino h3
    for (const tag of ['h1', 'h2', 'h3']) {
        const re = new RegExp(`<${tag}[^>]*>([\\s\\S]*?)<\\/${tag}>`, 'i');
        const m = article.match(re);
        if (m) {
            const t = stripTagsInline(m[1]);
            if (t) return t;
        }
    }
    return '';
}

// ====================================================================
// MAIN
// ====================================================================

function main() {
    if (!fs.existsSync(SRC_DIR)) {
        console.error(`No existe el directorio: ${SRC_DIR}`);
        console.error(`Ejecuta primero: node convert_manual.js --download`);
        process.exit(1);
    }
    if (!fs.existsSync(OUT_DIR)) fs.mkdirSync(OUT_DIR, { recursive: true });

    const files = fs.readdirSync(SRC_DIR).filter(f => f.endsWith('.html')).sort();
    const index = [];
    let totalChars = 0;

    for (const file of files) {
        const filePath = path.join(SRC_DIR, file);
        const html = readFile(filePath);

        const article = extractArticle(html);
        if (!article) {
            console.warn(`  [skip] ${file}: sin <article>`);
            continue;
        }

        const title = extractTitle(html);
        const md = htmlToMarkdown(article);
        const slug = file.replace(/\.html$/, '');
        const outPath = path.join(OUT_DIR, slug + '.md');
        fs.writeFileSync(outPath, md, 'utf8');

        totalChars += md.length;
        index.push({
            uri: 'manual://' + slug,
            name: title || slug,
            file: slug + '.md',
            size: md.length,
            source: 'https://conocimiento.blob.core.windows.net/conocimiento/Manuales/MR_SDK/' + file
        });
        console.log(`  [ok] ${file} -> ${slug}.md (${md.length} chars) [${title || '(sin titulo)'}]`);
    }

    fs.writeFileSync(path.join(OUT_DIR, 'index.json'), JSON.stringify(index, null, 2), 'utf8');
    console.log(`\nTotal: ${index.length} capítulos, ${totalChars} caracteres`);
}

// Entry point
const arg = process.argv[2];
if (arg === '--download' || arg === '-d') {
    downloadAll()
        .then(() => main())
        .catch((err) => {
            console.error('Error:', err.message);
            process.exit(1);
        });
} else {
    main();
}
