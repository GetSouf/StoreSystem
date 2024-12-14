document.addEventListener("DOMContentLoaded", () => {
    const categoryFilter = document.getElementById("category-filter");
    const productList = document.getElementById("product-list");
    const cart = document.getElementById("cart");
    const totalPriceElement = document.getElementById("total-price");
    const createOrderBtn = document.getElementById("create-order-btn");
    const customerSelect = document.getElementById("customer-select");

    // Хранилище товаров в корзине
    const cartItems = {};

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

        // Обработчики кнопок "+" и "-"
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

    // Создание заказа
    createOrderBtn.addEventListener('click', () => {
        const customerId = customerSelect.value;
        if (!customerId) {
            alert('Пожалуйста, выберите покупателя.');
            return;
        }

        const employeeId = 1;
        // Функция для получения текущего пользователя
        function fetchCurrentUser() {
            try {
                const response =  fetch('/Account/GetCurrentUser');
                if (!response.ok) {
                    throw new Error( response.text());
                }

                const data =  response.json();
                employeeId = data.EmployeeId; // Сохраняем EmployeeId текущего пользователя
                console.log(`Текущий сотрудник: ${data.Username} (ID: ${employeeId}, Роль: ${data.Role})`);
            } catch (error) {
                console.error("Ошибка при получении данных о текущем пользователе:", error.message);
                alert("Не удалось получить данные о текущем пользователе. Попробуйте обновить страницу.");
            }
        }

        // Вызов функции получения текущего пользователя при загрузке
        fetchCurrentUser();

        // ID текущего сотрудника (поменяй на реальную логику получения)
        const orderDetails = Object.entries(cartItems).map(([id, item]) => ({
            ProductId: parseInt(id, 10),
            Quantity: item.quantity,
            Price: item.price
        }));

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
                } else {
                    alert('Ошибка при создании заказа.');
                }
            });
    });
});
