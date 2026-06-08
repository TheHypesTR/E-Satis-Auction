# fenasal esatış dehşet satış vahşet satış çok iyi satış satış fenasal satış harika satışlar çok iyi satışlar ucuza kampanya satışlar yardım edin satıyorum çok fena satıyorum satışıyorum e-satıyorum hem de çok ucuza e-satıyorum deliler gibi e-satıyorum fenasal e-satıyorum yardım edin satıyorum çay satıyorum kahve satıyorum seni de satıyorum beni de satıyorum satamıyorum satamayınca ağlıyorum mağara adamlarıyla satışıyorum

# neyse beyler şaka bi yana kalan mock verier şunlar: Order, OrderItem, Id, OrderNumber (Sipariş No), CustomerName (veya UserId), TotalAmount, Date, Status (Bekliyor, Onaylandı, Kargolandı, İptal, Teslim Edildi), GET /api/Order (Admin için tüm siparişler), GET /api/Order/MyOrders (Kullanıcının kendi siparişleri), PUT /api/Order/{id}/Status (Durum güncelleme - Onaylandı/Kargolandı)

# iade için de şunlar lazım: ReturnRequest, Id, OrderId, CustomerName, Reason (İade sebebi), Date, Status (Bekliyor, Onaylandı, Reddedildi). POST /api/ReturnRequest (Yeni iade talebi), GET /api/ReturnRequest/MyReturns (Kullanıcının iadeleri), PUT /api/ReturnRequest/{id}/Status (İade durumu güncelleme - Onaylandı/Reddedildi)

