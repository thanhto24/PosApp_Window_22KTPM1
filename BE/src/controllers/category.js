const Category = require("../models/Category");

// Lấy tất cả danh mục
exports.getAllCategories = async (req, res) => {
    try {
        const categories = await Category.find();
        console.log(categories);
        res.json(categories);
    } catch (error) {
        res.status(500).json({ error: "Lỗi khi lấy danh mục" });
    }
};

// Thêm danh mục mới
exports.addCategory = async (req, res) => {
    try {
        const { name } = req.body;
        if (!name) return res.status(400).json({ error: "Tên không được để trống" });

        const newCategory = new Category({ name });
        await newCategory.save();
        res.json({ message: "Thêm thành công!", category: newCategory });
    } catch (error) {
        res.status(500).json({ error: "Lỗi khi thêm danh mục" });
    }
};
