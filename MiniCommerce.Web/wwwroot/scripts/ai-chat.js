(function () {
    const userGuidInput = document.getElementById('ai-user-guid-input');
    const promptInput = document.getElementById('ai-prompt-input');
    const chatLog = document.getElementById('ai-chat-log');
    const form = document.getElementById('ai-prompt-form');

    function appendMessage(role, text) {
        const entry = document.createElement('div');
        entry.textContent = `${role}: ${text}`;
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
            appendMessage('Assistant', data.response);
        } catch (err) {
            appendMessage('Error', 'Unable to reach the server.');
        } finally {
            promptInput.disabled = false;
            promptInput.focus();
        }
    });
})();
