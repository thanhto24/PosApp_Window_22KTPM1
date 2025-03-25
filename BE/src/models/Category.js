const mongoose = require("mongoose");

const CategorySchema = new mongoose.Schema({
    Name: { type: String, required: true },
});

module.exports = mongoose.model("category_", CategorySchema);
