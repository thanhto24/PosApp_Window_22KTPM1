const mongoose = require('mongoose');

const OrderSchema = new mongoose.Schema({
    InvoiceCode: { type: String, required: true },
    Customer: { type: String, required: true },
    SaleDateTime: { type: String, required: true },
    Salesperson: { type: String, required: true },
    TotalAmount: { type: Number, required: true },
    TotalDiscount: { type: Number, required: true },
    TotalPayment: { type: Number, required: true },
    TotalCost: { type: Number, required: true },
    PaymentMethod: { type: String, required: true },
    Status: { type: String, required: true },
    PaymentStatus: { type: String, required: true },
    Notes: { type: String, required: false }
}, { timestamps: true });

const Order = mongoose.model('order_', OrderSchema);
module.exports = Order;
