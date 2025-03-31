const category_ = require("./Category");
const order_ = require("./Order");
const voucher = require("./Voucher");
const product = require("./Product")
const customer = require("./Customer");
const orderedProduct = require("./OrderedProduct");
//Vi cateogry la tu khoa trong csdl, js,... nen phai them dau _ de tranh trung voi tu khoa do
//Vi order la tu khoa trong csdl, js,... nen phai them dau _ de tranh trung voi tu khoa do
//Chu y thong nhat ten cac model phai viet thuong, khong viet hoa
const models = { category_, order_, voucher, product, customer, orderedProduct };

module.exports = models;
