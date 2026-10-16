(function () {
    const userGuidInput = document.getElementById('ai-user-guid-input');
    const promptInput = document.getElementById('ai-prompt-input');
    const chatLog = document.getElementById('ai-chat-log');
    const form = document.getElementById('ai-prompt-form');
    const allowedMarkdownTags = new Set([
        'a', 'blockquote', 'br', 'code', 'del', 'em', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
        'hr', 'img', 'input', 'li', 'ol', 'p', 'pre', 's', 'strong', 'table', 'tbody', 'td',
        'th', 'thead', 'tr', 'ul'
    ]);

    if (!userGuidInput || !promptInput || !chatLog || !form) {
        return;
    }

    function safeUrl(value, allowMailto) {
        try {
            const url = new URL(value, document.baseURI);
            if (url.protocol === 'http:' || url.protocol === 'https:' || (allowMailto && url.protocol === 'mailto:')) {
                return url.href;
            }
        } catch (err) {
            return null;
        }

        return null;
    }

    function appendSanitizedNode(source, parent) {
        if (source.nodeType === Node.TEXT_NODE) {
            parent.appendChild(document.createTextNode(source.nodeValue || ''));
            return;
        }

        if (source.nodeType !== Node.ELEMENT_NODE) {
            return;
        }

        const tag = source.tagName.toLowerCase();
        if (tag === 'script' || tag === 'style' || tag === 'iframe' || tag === 'object' || tag === 'embed' || tag === 'svg' || tag === 'math') {
            return;
        }

        if (tag === 'input' && source.getAttribute('type') !== 'checkbox') {
            return;
        }

        if (!allowedMarkdownTags.has(tag)) {
            for (const child of source.childNodes) {
                appendSanitizedNode(child, parent);
            }
            return;
        }

        const element = document.createElement(tag);
        if (tag === 'a') {
            const rawHref = source.getAttribute('href');
            const href = rawHref ? safeUrl(rawHref, true) : null;
            if (href) {
                element.setAttribute('href', href);
                element.setAttribute('rel', 'noopener noreferrer');
            }
            if (source.hasAttribute('title')) {
                element.setAttribute('title', source.getAttribute('title'));
            }
        } else if (tag === 'img') {
            const rawSrc = source.getAttribute('src');
            const src = rawSrc ? safeUrl(rawSrc, false) : null;
            if (!src) {
                parent.appendChild(document.createTextNode(source.getAttribute('alt') || ''));
                return;
            }
            element.setAttribute('src', src);
            element.setAttribute('alt', source.getAttribute('alt') || '');
            element.setAttribute('loading', 'lazy');
            element.setAttribute('referrerpolicy', 'no-referrer');
        } else if (tag === 'input') {
            element.setAttribute('type', 'checkbox');
            element.disabled = true;
            element.checked = source.hasAttribute('checked');
        } else if (tag === 'code') {
            const className = source.getAttribute('class') || '';
            if (/^language-[\w-]+$/.test(className)) {
                element.className = className;
            }
        }

        for (const child of source.childNodes) {
            appendSanitizedNode(child, element);
        }
        parent.appendChild(element);
    }

    function appendMarkdown(container, text) {
        if (!window.marked || typeof window.marked.parse !== 'function') {
            container.textContent = text;
            return;
        }

        const template = document.createElement('template');
        template.innerHTML = window.marked.parse(text);
        for (const node of template.content.childNodes) {
            appendSanitizedNode(node, container);
        }
    }

    function appendMessage(role, text, isMarkdown) {
        const entry = document.createElement('div');
        const label = document.createElement('strong');
        label.textContent = `${role}: `;
        entry.appendChild(label);

        if (isMarkdown) {
            const content = document.createElement('div');
            appendMarkdown(content, text);
            entry.appendChild(content);
        } else {
            entry.appendChild(document.createTextNode(text));
        }

        chatLog.appendChild(entry);
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const prompt = promptInput.value.trim();
        if (!prompt) {
            return;
        }

        const userGuid = userGuidInput.value;
        const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

        appendMessage('You', prompt);
        promptInput.value = '';
        promptInput.disabled = true;

        try {
            const response = await fetch('?handler=Send', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({ prompt, userGuid })
            });

            if (!response.ok) {
                appendMessage('Error', `Request failed (${response.status}).`);
                return;
            }

            const data = await response.json();
            appendMessage('Assistant', data.response || '', true);
            appendMessage('(Token Usage', `Input: ${data.inputToken}, Output: ${data.outputToken}, Reasoning: ${data.reasoningToken}, Total: ${data.totalToken})`);
        } catch (err) {
            appendMessage('Error', 'Unable to reach the server.');
        } finally {
            promptInput.disabled = false;
            promptInput.focus();
        }
    });
})();
