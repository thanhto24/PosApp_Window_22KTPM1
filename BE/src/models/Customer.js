const mongoose = require('mongoose');

const CustomerSchema = new mongoose.Schema({
    Name: { type: String, required: true }, // Tên khách hàng
    Phone_num: { type: String, required: true }, // Số điện thoại
    AmountOrder: { type: Number, required: true }, // Số đơn hàng đã đặt
    TotalPaid: { type: Number, required: true }, // Tổng số tiền đã thanh toán
    Rank: { type: String, required: true } // Hạng khách hàng
});

const Customer = mongoose.model('customer', CustomerSchema, 'customer');
module.exports = Customer;
