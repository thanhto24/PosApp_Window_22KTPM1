const mongoose = require("mongoose");

const OrderedProductSchema = new mongoose.Schema({
    ProductCode: { type: String, required: false },
    Name: { type: String, required: true },
    Quantity: { type: Number, required: true },
    Price: { type: Number, required: true }
});

const OrderSchema = new mongoose.Schema(
    {
        InvoiceCode: { type: String, required: true },
        Customer: { type: String, required: true },
        SaleDateTime: { type: Date, required: true, default: Date.now },
        OrderedProducts: { type: [OrderedProductSchema], required: true }, // Danh sách sản phẩm
        TotalAmount: { type: Number, required: true },
        TotalDiscount: { type: Number, required: true },
        TotalPayment: { type: Number, required: true },
        TotalCost: { type: Number, required: true },
        PaymentMethod: { type: String, required: true },
        Status: { type: String, required: true },
        PaymentStatus: { type: String, required: true },
        Notes: { type: String, default: "" }
    },
    { timestamps: true, collection: "order_" } // Đổi collection thành "orders" (số nhiều)
);

const Order = mongoose.model("Order", OrderSchema);
module.exports = Order;
