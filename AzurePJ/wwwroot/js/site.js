(function () {
    function createFloatingHeart(x, y) {
        const heart = document.createElement('div');
        heart.className = 'heart';
        heart.innerText = '❤';
        heart.style.left = (x - 10) + 'px';
        heart.style.top = (y - 10) + 'px';
        document.body.appendChild(heart);
        setTimeout(() => heart.remove(), 2000);
    }

    document.addEventListener('click', e => createFloatingHeart(e.clientX, e.clientY));
    document.addEventListener('touchstart', e => {
        if (e.touches && e.touches[0]) {
            createFloatingHeart(e.touches[0].clientX, e.touches[0].clientY);
        }
    });
})();

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

document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".sidebar");
    const toggleButton = document.getElementById("sidebarToggle");
    const backdrop = document.getElementById("sidebarBackdrop");

    if (toggleButton && sidebar && backdrop) {
        toggleButton.addEventListener("click", () => {
            sidebar.classList.toggle("active");
            backdrop.classList.toggle("active");
        });

        backdrop.addEventListener("click", () => {
            sidebar.classList.remove("active");
            backdrop.classList.remove("active");
        });
    }
});
