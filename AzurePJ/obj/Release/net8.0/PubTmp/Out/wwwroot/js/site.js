(function () {
    function createFloatingHeart(x, y) {
        const heart = document.createElement('div');
        heart.className = 'heart';
        heart.innerText = '❤';
        document.body.appendChild(heart);

        heart.style.left = (x - 10) + 'px';
        heart.style.top = (y - 10) + 'px';

        setTimeout(() => heart.remove(), 2000);
    }

    function handleClickOrTouch(e) {
        const x = e.clientX || (e.touches && e.touches[0].clientX);
        const y = e.clientY || (e.touches && e.touches[0].clientY);
        if (x && y) createFloatingHeart(x, y);
    }

    document.addEventListener('click', handleClickOrTouch);
    document.addEventListener('touchstart', handleClickOrTouch);
})();
(function () {
    window.initLoveButton = function (btnSelector) {
        const btn = document.querySelector(btnSelector);
        if (!btn) return;

        btn.addEventListener('click', function () {
            for (let i = 0; i < 5; i++) {
                setTimeout(() => {
                    const heart = document.createElement('div');
                    heart.className = 'heart';
                    heart.innerText = '❤';
                    btn.appendChild(heart);

                    heart.style.left = (Math.random() * btn.offsetWidth) + 'px';
                    heart.style.top = '-10px';
                    heart.style.position = 'absolute';
                    heart.style.fontSize = '16px';

                    setTimeout(() => heart.remove(), 2000);
                }, i * 150);
            }
        });
    };
})();