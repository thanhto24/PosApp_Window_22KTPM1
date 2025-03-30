const mongoose = require('mongoose');

const OrderedProductSchema = new mongoose.Schema({
    productCode: { type: String, required: false, unique: true },
    name: { type: String, required: true },
    quantity: { type: Number, required: true },
    price: { type: Number, required: true },
});

const OrderedProduct = mongoose.model('orderedProduct', OrderedProductSchema, 'orderedProduct');
module.exports = OrderedProduct;
