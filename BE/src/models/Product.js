const mongoose = require('mongoose');

const ProductSchema = new mongoose.Schema({
    Name: { type: String, required: true },
    Price: { type: Number, required: true },
    ImagePath: { type: String, required: false },
    BarCode: { type: String, required: true, unique: true },
    TypeGroup: { type: String, required: true },
    VAT: { type: Number, required: true },
    CostPrice: { type: Number, required: true }
});

const Product = mongoose.model('product', ProductSchema, 'product');
module.exports = Product;