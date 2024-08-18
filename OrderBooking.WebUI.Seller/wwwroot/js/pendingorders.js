class PendingOrders extends HTMLElement{
    constructor(){
        super();

        this.innerHTML = `<table style='border-collapse: colllapse; width: 60%'>
                        <tbody>
                            <tr style='padding-left:0;text-align:left'>
                                <th>Order Id</th>
                                <th>Order name</th>
                                <th>Order status</th>
                                <th>Order amount</th>
                                <th>Action</th>
                            </tr>
                        </tbody>
                        <tbody id="orders"></tbody>
                        </table>`;

    this.rowTemplate = `<tr>
                            <td id="id"></td>
                            <td class="text-name"></td>
                            <td class="text-status"></td>
                            <td class="text-amount"></td>
                            <td><confirm-sales-order></confirm-sales-order></td>
                        </tr>`;
    }

    async connectedCallback(){
        if (!this.salesOrders) {
            this.salesOrders = await this.load();
        }
    
        this.render();
    }

    async load(){
        var url = "http://localhost:5291/api/pendingOrders";
        var response = await fetch(url, {
            method: "Get",
            mode: "cors",
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (response.status == 200) {
            return await response.json();
        }
        else {
            return null;
        }
    }

    render() {
        var table = this.querySelector("#orders");
        table.innerHTML = "";
    
        for (var order of this.salesOrders)
        {
            var row = this.htmlToElement(this.rowTemplate);
    
            var id = row.querySelector("#id");
            id.innerHTML = order.id;
            
            var name = row.querySelector(".text-name");
            name.innerHTML = order.name;

            var status = row.querySelector(".text-status");
            status.innerHTML = order.status;

            var amount = row.querySelector(".text-amount");
            amount.innerHTML = order.amount;

            var action = row.querySelector("confirm-sales-order");
            action.setAttribute("data-order-id", order.id);
            action.addEventListener("confirmed", () => row.remove());
    
            table.append(row);
        }
    }
    htmlToElement(html) {
        var template = document.createElement('template');
        html = html.trim(); 
        template.innerHTML = html;
        return template.content.firstChild;
    }
}

customElements.define("pending-orders", PendingOrders);