const mongoose = require('mongoose');

const ProductSchema = new mongoose.Schema({
    Name: { type: String, required: true },
    Price: { type: Number, required: true },
    ImagePath: { type: String, required: false },
    BarCode: { type: String, required: true, unique: true },
    TypeGroup: { type: String, required: true },
    Vat: { type: Number, required: false },
    CostPrice: { type: Number, required: true },

    //tồn kho:
    Inventory: {type: Number, required: true},
});

const Product = mongoose.model('product', ProductSchema, 'product');
module.exports = Product;