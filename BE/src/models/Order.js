const mongoose = require('mongoose');

const OrderSchema = new mongoose.Schema({
    InvoiceCode: { type: String, required: true },
    Customer: { type: String, required: true },
    SaleDateTime: { type: Date, required: true }, // Chuyển sang Date để dễ thao tác
    TotalAmount: { type: Number, required: true },
    TotalDiscount: { type: Number, required: true },
    TotalPayment: { type: Number, required: true },
    TotalCost: { type: Number, required: true },
    PaymentMethod: { type: String, required: true },
    Status: { type: String, required: true },
    PaymentStatus: { type: String, required: true },
    Notes: { type: String, default: "" } // Fix lỗi null
}, { timestamps: true, collection: "order_" });

const Order = mongoose.model("order", OrderSchema);
module.exports = Order;
