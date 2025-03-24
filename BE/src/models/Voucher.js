const mongoose = require('mongoose');

const VoucherSchema = new mongoose.Schema({
    Code: { type: String, required: true, unique: true }, // Mã giảm giá
    Quantity: { type: Number, required: true }, // Số lượng voucher
    Note: { type: String, required: false }, // Ghi chú (nếu có)
    MinOrder: { type: Number, required: true }, // Giá trị đơn hàng tối thiểu
    DiscountValue: { type: Number, required: true }, // Giá trị giảm giá
    StartDate: { type: Date, required: false }, // Ngày bắt đầu (nullable)
    EndDate: { type: Date, required: false } // Ngày kết thúc (nullable)
});

const Voucher = mongoose.model('voucher', VoucherSchema, 'voucher');
module.exports = Voucher;
