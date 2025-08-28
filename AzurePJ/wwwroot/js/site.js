(function () {
    // ===== Hearts effect =====
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

    // ===== Sidebar toggle (mobile) =====
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

            const mq = window.matchMedia("(min-width: 769px)");
            const handle = e => {
                if (e.matches) {
                    sidebar.classList.remove("active");
                    backdrop.classList.remove("active");
                }
            };
            if (mq.addEventListener) mq.addEventListener("change", handle);
            else mq.addListener(handle);
        }

        // ===== Local file preview =====
        const localFilesInput = document.getElementById("localFiles");
        if (localFilesInput) {
            localFilesInput.addEventListener("change", function () {
                const preview = document.getElementById("localPreview");
                preview.innerHTML = "";
                Array.from(this.files).forEach(file => {
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const img = document.createElement("img");
                        img.src = e.target.result;
                        img.className = "preview-img-thumb";
                        preview.appendChild(img);
                    };
                    reader.readAsDataURL(file);
                });
            });
        }

        // ===== Photo modal preview =====
        const modal = document.getElementById("previewModal");
        const imgEl = document.getElementById("previewImg");
        const titleEl = document.getElementById("previewTitle");
        const closeBtn = modal ? modal.querySelector(".close-btn") : null;
        const prevBtn = modal ? modal.querySelector(".prev") : null;
        const nextBtn = modal ? modal.querySelector(".next") : null;

        let currentIndex = 0;
        window.photoData = window.photoData || [];

        function showSlide(index) {
            currentIndex = index;
            const photo = window.photoData[currentIndex];
            if (!photo) return;
            imgEl.src = photo.url;
            titleEl.innerText = photo.title || "";
            modal.style.display = "block";
        }

        function closePreview() {
            if (modal) modal.style.display = "none";
        }

        function changeSlide(n) {
            if (!window.photoData.length) return;
            currentIndex += n;
            if (currentIndex < 0) currentIndex = window.photoData.length - 1;
            if (currentIndex >= window.photoData.length) currentIndex = 0;
            showSlide(currentIndex);
        }

        // 缩略图绑定点击事件
        document.querySelectorAll(".preview-img-item").forEach(item => {
            item.addEventListener("click", e => {
                const index = parseInt(item.getAttribute("data-index"));
                showSlide(index);
            });
        });

        // 弹窗按钮事件
        if (closeBtn) closeBtn.addEventListener("click", closePreview);
        if (prevBtn) prevBtn.addEventListener("click", () => changeSlide(-1));
        if (nextBtn) nextBtn.addEventListener("click", () => changeSlide(1));

        // 键盘支持
        document.addEventListener("keydown", e => {
            if (!modal || modal.style.display !== "block") return;
            if (e.key === "ArrowLeft") changeSlide(-1);
            if (e.key === "ArrowRight") changeSlide(1);
            if (e.key === "Escape") closePreview();
        });

        // 保留全局函数
        window.openPreview = function (url, title) {
            if (!modal) return;
            imgEl.src = url;
            titleEl.innerText = title || "";
            modal.style.display = "block";
        };
        window.closePreview = closePreview;
    });

    // ===== Optional love button effect =====
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
})();
