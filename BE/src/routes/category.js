const express = require("express");
const { getAllCategories, addCategory } = require("../controllers/category");

const router = express.Router();

router.get("/getall", getAllCategories);
router.post("/add", addCategory);

module.exports = router;
