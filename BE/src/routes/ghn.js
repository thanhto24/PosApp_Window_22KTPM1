const express = require('express');
const router = express.Router();
const ghnController = require('../controllers/GHNController');

router.post('/ship-fee', ghnController.calculateShippingFee);
router.post('/create-order', ghnController.createOrder);


router.get('/provinces', ghnController.getProvinces);
router.get('/districts', ghnController.getDistrictsByProvince);
router.get('/wards', ghnController.getWardsByDistrict);

module.exports = router;
