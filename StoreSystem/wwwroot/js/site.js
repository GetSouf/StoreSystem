document.addEventListener("DOMContentLoaded", () => {
    const categoryFilter = document.getElementById("category-filter");
    const productList = document.getElementById("product-list");
    const cart = document.getElementById("cart");
    const totalPriceElement = document.getElementById("total-price");
    const createOrderBtn = document.getElementById("create-order-btn");
    const customerSelect = document.getElementById("customer-select");
    const steps = document.querySelectorAll(".step");
    const prevBtn = document.getElementById("prev-step-btn");
    const nextBtn = document.getElementById("next-step-btn");

    let currentStep = 0;

    const cartItems = {};
    function updateSteps() {
        steps.forEach((step, index) => {
            step.style.display = index === currentStep ? "block" : "none";
            step.classList.toggle("active", index === currentStep);
        });
        prevBtn.style.display = currentStep === 0 ? "none" : "inline-block";
        nextBtn.style.display = currentStep === steps.length - 1 ? "none" : "inline-block";
        if (currentStep == 3) {
            updateOrderSummary();
            currentStep = 0; 
            updateSteps();

        }
    }
    
    

    // События кнопок
    prevBtn.addEventListener("click", () => {
        if (currentStep > 0) currentStep--;
        updateSteps();
    });

    nextBtn.addEventListener("click", () => {
        if (currentStep < steps.length - 1) currentStep++;
        updateSteps();
    });


    // Изначально показать первый шаг
    updateSteps();


    // Функция обновления корзины
    function updateCart() {
        cart.innerHTML = '';
        let totalPrice = 0;

        for (const [id, item] of Object.entries(cartItems)) {
            const cartItem = document.createElement('div');
            cartItem.classList.add('cart-item');
            cartItem.innerHTML = `
                <span>${item.name}</span>
                <span>${item.price} ₽</span>
                <span>
                    <button class="btn btn-secondary btn-sm decrease" data-id="${id}">-</button>
                    <strong>${item.quantity}</strong>
                    <button class="btn btn-secondary btn-sm increase" data-id="${id}">+</button>
                </span>
                <span class="item-total">${(item.price * item.quantity).toFixed(2)} ₽</span>
            `;
            cart.appendChild(cartItem);

            totalPrice += item.price * item.quantity;
        }

        totalPriceElement.textContent = totalPrice.toFixed(2);

        document.querySelectorAll('.decrease').forEach(button => {
            button.addEventListener('click', () => {
                const id = button.dataset.id;
                if (cartItems[id].quantity > 1) {
                    cartItems[id].quantity--;
                    updateStock(id, 1);
                } else {
                    delete cartItems[id];
                }
                updateCart();
            });
        });

        document.querySelectorAll('.increase').forEach(button => {
            button.addEventListener('click', () => {
                const id = button.dataset.id;
                const stock = parseInt(document.getElementById(`stock-${id}`).textContent, 10);
                if (cartItems[id].quantity < stock) {
                    cartItems[id].quantity++;
                    updateStock(id, -1);
                } else {
                    alert('Недостаточно товара на складе.');
                }
                updateCart();
            });
        });
    }
    

    // Обновление количества товара на складе
    function updateStock(id, change) {
        const stockElement = document.getElementById(`stock-${id}`);
        const stock = parseInt(stockElement.textContent, 10);
        stockElement.textContent = stock + change;
    }

    // Обработка смены категории
    categoryFilter.addEventListener('change', () => {
        const categoryId = categoryFilter.value;

        if (!categoryId) {
            productList.innerHTML = '<p>Выберите категорию, чтобы увидеть товары.</p>';
            return;
        }

        fetch(`/Sale/GetProductsByCategory?categoryId=${categoryId}`)
            .then(response => response.json())
            .then(products => {
                productList.innerHTML = '';

                if (products.length === 0) {
                    productList.innerHTML = '<p>В этой категории нет товаров.</p>';
                    return;
                }

                products.forEach(product => {
                    const productCard = document.createElement('div');
                    productCard.classList.add('product-card');
                    productCard.innerHTML = `
                        <h5>${product.name}</h5>
                        <p>Цена: ${product.price} ₽</p>
                        <p>В наличии: <span id="stock-${product.id}">${product.stockQuantity}</span></p>
                        <button class="btn btn-success" data-id="${product.id}" data-name="${product.name}" data-price="${product.price}" data-stock="${product.stockQuantity}">Добавить в корзину</button>
                    `;
                    productList.appendChild(productCard);
                });

                document.querySelectorAll('.product-card .btn-success').forEach(button => {
                    button.addEventListener('click', () => {
                        const id = button.dataset.id;
                        const name = button.dataset.name;
                        const price = parseFloat(button.dataset.price);
                        const stock = parseInt(button.dataset.stock, 10);

                        if (!cartItems[id]) {
                            cartItems[id] = { name, price, quantity: 0 };
                        }

                        if (cartItems[id].quantity < stock) {
                            cartItems[id].quantity++;
                            updateStock(id, -1);
                            updateCart();
                        } else {
                            alert(`Вы не можете добавить больше, чем ${stock} шт.`);
                        }
                    });
                });
            });
    });
    async function fetchCurrentUser() {
        try {
            const response = await fetch('/Account/GetCurrentUser');
            if (!response.ok) {
                throw new Error(await response.text());
            }

            const data = await response.json();
            console.log("Ответ от сервера:", data);
            return data.employeeId; // Возвращаем ID сотрудника
        } catch (error) {
            console.error("Ошибка при получении данных о текущем пользователе:", error.message);
            alert("Не удалось получить данные о текущем пользователе. Попробуйте обновить страницу.");
            return null;
        }
    }
    
    createOrderBtn.addEventListener('click', async () => { // Добавляем async здесь
        const customerId = customerSelect.value;
        if (!customerId) {
            alert('Пожалуйста, выберите покупателя.');
            return;
        }

        // Получаем ID сотрудника асинхронно
        const employeeId = await fetchCurrentUser();
        if (!employeeId) {
            alert("Не удалось получить ID сотрудника. Попробуйте позже.");
            return;
        }

        // Формируем детали заказа
        const orderDetails = Object.entries(cartItems).map(([id, item]) => ({
            ProductId: parseInt(id, 10),
            Quantity: item.quantity,
            Price: item.price
        }));



        // Отправляем заказ на сервер
        fetch('/api/CreateOrder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ customerId, employeeId, orderDetails })


        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text); });
                }
                return response.json();
            })
            .then(data => {
                if (data.orderId) {
                    alert(`Заказ #${data.orderId} успешно создан!`);
                    Object.keys(cartItems).forEach(id => updateStock(id, cartItems[id].quantity));
                    Object.keys(cartItems).forEach(id => delete cartItems[id]);
                    updateCart();
                    location.reload(); 
                } else {
                    alert('Ошибка при создании заказа.');
                }
            })
            .catch(error => {
                console.error("Ошибка при создании заказа:", error.message);
                alert("Произошла ошибка при создании заказа.");
            });
    });
    function updateOrderSummary() {
        const cartSummary = document.getElementById("cart-summary");
        const cartTotal = document.getElementById("cart-total");

        cartSummary.innerHTML = '';
        let total = 0;

        if (Object.keys(cartItems).length === 0) {
            cartSummary.innerHTML = '<p>Ваша корзина пуста.</p>';
        } else {
            for (const [id, item] of Object.entries(cartItems)) {
                const cartRow = document.createElement('div');
                cartRow.classList.add('cart-summary-item');
                cartRow.innerHTML = `
                <span>${item.name}</span>
                <span>${item.quantity} шт.</span>
                <span>${(item.price * item.quantity).toFixed(2)} ₽</span> `;
                cartSummary.appendChild(cartRow);

                total += item.price * item.quantity;
            }
        }

        cartTotal.textContent = total.toFixed(2);
    }
    function changeOrderStatus(orderId, newStatus) {
        fetch(`/Sale/ChangeOrderStatus?orderId=${orderId}&newStatus=${newStatus}`, {
            method: 'POST'
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text); });
                }
                return response.json();
            })
            .then(data => {
                alert(data.message);
                location.reload(); 
            })
            .catch(error => {
                console.error("Ошибка изменения статуса:", error.message);
                alert("Произошла ошибка при изменении статуса.");
            });
    }
   
});
