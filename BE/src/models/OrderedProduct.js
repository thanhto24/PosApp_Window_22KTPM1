const mongoose = require('mongoose');

const OrderedProductSchema = new mongoose.Schema({
    ProductCode: { type: String, required: true, unique: true },
    Name: { type: String, required: true },
    Quantity: { type: Number, required: true },
    Price: { type: Number, required: true },
});

const OrderedProduct = mongoose.model('orderedProduct', OrderedProductSchema, 'orderedProduct');
module.exports = OrderedProduct;
