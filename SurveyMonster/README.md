# Survey Monster | Anket Projesi
baseUrl: https://test-api.elearningsolutions.net/swagger/index.html

**Description**
Kullanıcılar auth (giriş yapmış)  şekilde güncel ankete katılacaklardır. Tüm Database işlemleri API üzerinden sağlanacaktır.

Auth olan kullanıcılar için API isteklerinde token gönderilmesi zorunludur.

`API_SPEC.md` dosyasında  tüm API request & response yapıları detaylandırılacaktır.

baseUrl => appsettingsden çek: "http://test-api.elearningsolutions.net"
---

## 🔧 Teknoloji & Mimari

* JWT Authentication
* DI (Dependency Injection) Servis Mimarisine göre yazılacaktır

---

## 🎯 Özellikler

* Giriş yapmış kullanıcılarda token doğrulaması
* API üzerinden aktif anket gösterimi
* Ankete seçenek ekleme (Admin)
* Kullanıcı başına tek oy kontrolü (token veya IP,UserAgent)

---

## 📌 API Dokümantasyonu

Endpointler aşağıda belirtilen dosyadadır:

➡️ `API_SPEC.md`

---

## 🗂️ Klasör Yapısı

```bash
root
│   README.md
│
└───Specs
    API_SPEC.md
```

---

## ✅ Gereksinimler

* Backend: API tüm database işlemlerini yönetecek
* Frontend: API servislerini çağırarak çalışacak
* Her servis DI'a uygun yazılacak
