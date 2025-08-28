// Hearts effect
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


    if (!window.photoData) window.photoData = [];

    const modal = document.getElementById("previewModal");
    const imgEl = document.getElementById("previewImg");
    const titleEl = document.getElementById("previewTitle");
    const closeBtn = modal.querySelector(".close-btn");
    const prevBtn = modal.querySelector(".prev");
    const nextBtn = modal.querySelector(".next");

    let currentIndex = 0;

    function showSlide(index) {
        currentIndex = index;
        const photo = window.photoData[currentIndex];
        if (!photo) return;
        imgEl.src = photo.url;
        titleEl.innerText = photo.title;
        modal.style.display = "block";
    }

    function closePreview() {
        modal.style.display = "none";
    }

    $('#sidebarToggle').click(function () {
        $('#sidebar').toggleClass('active');
        $('#sidebarBackdrop').toggleClass('active');
    });
    $('#sidebarBackdrop').click(function () {
        $('#sidebar').removeClass('active');
        $('#sidebarBackdrop').removeClass('active');
    });

    function changeSlide(n) {
        currentIndex += n;
        if (currentIndex < 0) currentIndex = window.photoData.length - 1;
        if (currentIndex >= window.photoData.length) currentIndex = 0;
        showSlide(currentIndex);
    }

    // 点击缩略图
    document.querySelectorAll(".preview-img-item").forEach(item => {
        item.addEventListener("click", e => {
            const index = parseInt(item.getAttribute("data-index"));
            showSlide(index);
        });
    });

    // 弹窗按钮
    closeBtn.addEventListener("click", closePreview);
    prevBtn.addEventListener("click", () => changeSlide(-1));
    nextBtn.addEventListener("click", () => changeSlide(1));

    // 键盘支持
    document.addEventListener("keydown", e => {
        if (modal.style.display !== "block") return;
        if (e.key === "ArrowLeft") changeSlide(-1);
        if (e.key === "ArrowRight") changeSlide(1);
        if (e.key === "Escape") closePreview();
    });

})();

window.initLoveButton = function (btnSelector) {
    const btn = document.querySelector(btnSelector);
    if (!btn) return;
    btn.style.position = 'relative';
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

// Sidebar toggle (mobile)
document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const toggle = document.getElementById("sidebarToggle");
    const backdrop = document.getElementById("sidebarBackdrop");

    if (toggle && sidebar && backdrop) {
        toggle.addEventListener("click", () => {
            sidebar.classList.toggle("active");
            backdrop.classList.toggle("active");
        });

        backdrop.addEventListener("click", () => {
            sidebar.classList.remove("active");
            backdrop.classList.remove("active");
        });

        // 当切回桌面端时，自动关闭抽屉状态
        const mq = window.matchMedia("(min-width: 769px)");
        const handle = e => { if (e.matches) { sidebar.classList.remove("active"); backdrop.classList.remove("active"); } };
        if (mq.addEventListener) mq.addEventListener("change", handle);
        else mq.addListener(handle); // 兼容旧浏览器
    }
});

function openPreview(url, title) {
    var modal = document.getElementById("previewModal");
    var img = document.getElementById("previewImg");
    var caption = document.getElementById("previewTitle");
    modal.style.display = "block";
    img.src = url;
    caption.innerText = title || "";
}

function closePreview() {
    var modal = document.getElementById("previewModal");
    modal.style.display = "none";
}

document.addEventListener("DOMContentLoaded", function () {
    var localFilesInput = document.getElementById("localFiles");
    if (localFilesInput) {
        localFilesInput.addEventListener("change", function () {
            var preview = document.getElementById("localPreview");
            preview.innerHTML = ""; // 清空上次预览
            Array.from(this.files).forEach(file => {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var img = document.createElement("img");
                    img.src = e.target.result;
                    img.className = "preview-img-thumb";
                    preview.appendChild(img);
                };
                reader.readAsDataURL(file);
            });
        });
    }
});

