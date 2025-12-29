# Flujo de la Aplicación - QPV Control Mercancias

## Diagrama de Flujo Principal

```
┌─────────────────────────────────────────────────────────────┐
│                        INICIO APP                            │
│                    (App.xaml.cs)                            │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    NavigationPage                            │
│                    ┌────────────┐                           │
│                    │ MainPage   │ ◄────┐                    │
│                    └────────────┘      │                    │
│                         │               │ Regreso           │
│    ┌───────────────────┼───────────┐   │                    │
│    ▼                   ▼           ▼   │                    │
│ ┌──────┐         ┌──────────┐  ┌─────────────┐            │
│ │Lista │         │ Escanear │  │ Ver Producto│            │
│ │Prods │         │ Barcode  │  │  (click)    │            │
│ └──────┘         └──────────┘  └─────────────┘            │
│                         │               │                    │
└─────────────────────────┼───────────────┼────────────────────┘
                          │               │
                          ▼               ▼
                  ┌──────────────┐ ┌──────────────┐
                  │ ScannerPage  │ │ProductDetails│
                  │              │ │    Page      │
                  └──────────────┘ └──────────────┘
                          │               │
                          │               │
                          ▼               ▼
                  ┌────────────────────────────┐
                  │   DatabaseService          │
                  │   (SQLite)                 │
                  └────────────────────────────┘
```

## Flujo Detallado: Escaneo de Código de Barras

```
Usuario en MainPage
        │
        │ [Toca "Escanear Código de Barras"]
        ▼
Navigation.PushAsync(ScannerPage)
        │
        ▼
┌─────────────────────────────────────────┐
│   ScannerPage - Cámara Activa          │
│                                         │
│   ┌──────────────────────────────┐    │
│   │  CameraBarcodeReaderView     │    │
│   │  - Auto-detecta códigos      │    │
│   │  - OnBarcodesDetected()      │    │
│   └──────────────────────────────┘    │
└─────────────────────────────────────────┘
        │
        │ [Código detectado]
        ▼
Vibration.Vibrate(200ms) ─── Feedback háptico
        │
        ▼
DatabaseService.GetProductByBarcodeAsync(barcode)
        │
        ├─────────┬──────────┐
        │         │          │
    ¿Existe?    SI         NO
        │         │          │
        ▼         ▼          ▼
        │  ┌──────────┐  ┌──────────┐
        │  │ Producto │  │ Nuevo    │
        │  │ Existe   │  │ Producto │
        │  └──────────┘  └──────────┘
        │         │          │
        └─────────┴──────────┘
                  │
                  ▼
        Navigation.PushAsync(
            ProductDetailsPage(product o barcode)
        )
```

## Flujo Detallado: Gestión de Producto

```
ProductDetailsPage
        │
        ├──────────┬──────────┐
        ▼          ▼          ▼
   ┌────────┐ ┌────────┐ ┌────────┐
   │ Ver    │ │ Editar │ │ Crear  │
   │ Foto   │ │ Datos  │ │ Nuevo  │
   └────────┘ └────────┘ └────────┘
        │          │          │
        │          │          ▼
        │          │    ┌──────────────┐
        │          │    │ Validaciones │
        │          │    │ - Barcode?   │
        │          │    │ - Descripción│
        │          │    │ - UdM?       │
        │          │    │ - Números    │
        │          │    └──────────────┘
        │          │          │
        │          │      ¿Válido?
        │          │          │
        │          │      ┌───┴───┐
        │          │      │       │
        │          │     SI       NO
        │          │      │       │
        │          │      │       ▼
        │          │      │   [Alert Error]
        │          │      │       │
        │          │      │       └─── Corregir
        │          │      │
        ▼          ▼      ▼
┌──────────────────────────────┐
│ Capturar Foto                │
│                              │
│ MediaPicker.CapturePhotoAsync│
│         │                    │
│         ▼                    │
│ Guardar en FileSystem        │
│ product_{GUID}.jpg           │
└──────────────────────────────┘
                │
                ▼
┌─────────────────────────────────┐
│ DatabaseService.SaveProductAsync│
│                                 │
│ ┌─────────────┐                │
│ │ product.Id? │                │
│ └─────────────┘                │
│       │                         │
│   ┌───┴───┐                    │
│   │       │                     │
│   0     >0                      │
│   │       │                     │
│   │       │                     │
│ INSERT  UPDATE                  │
│   │       │                     │
│   └───┬───┘                    │
└───────┼────────────────────────┘
        │
        ▼
[DisplayAlert "Éxito"]
        │
        ▼
Navigation.PopToRootAsync()
        │
        ▼
MainPage.OnAppearing()
        │
        ▼
LoadProducts() ─── Actualiza lista
```

## Flujo de Datos: Base de Datos

```
┌────────────────────────────────────────┐
│         DatabaseService                │
│                                        │
│  Singleton - Una instancia global     │
└────────────────────────────────────────┘
                  │
                  ▼
┌────────────────────────────────────────┐
│     InitializeAsync()                  │
│                                        │
│  ┌──────────────────────────────┐    │
│  │ Primera llamada:              │    │
│  │ - new SQLiteAsyncConnection  │    │
│  │ - CreateTableAsync<Product>  │    │
│  │                               │    │
│  │ Llamadas subsecuentes:        │    │
│  │ - Retorna inmediatamente      │    │
│  └──────────────────────────────┘    │
└────────────────────────────────────────┘
                  │
    ┌─────────────┼─────────────┐
    │             │             │
    ▼             ▼             ▼
┌────────┐  ┌─────────┐  ┌─────────┐
│ GET    │  │ SAVE    │  │ DELETE  │
│        │  │         │  │         │
│ SELECT │  │INSERT/  │  │ DELETE  │
│        │  │UPDATE   │  │         │
└────────┘  └─────────┘  └─────────┘
    │             │             │
    └─────────────┴─────────────┘
                  │
                  ▼
┌────────────────────────────────────────┐
│         products.db (SQLite)           │
│                                        │
│  Ubicación:                            │
│  - Android: /data/data/.../files/      │
│  - iOS: ~/Library/Application Support/ │
└────────────────────────────────────────┘
```

## Arquitectura de Capas

```
┌─────────────────────────────────────────────────────────┐
│                    PRESENTACIÓN                         │
│                                                         │
│  ┌────────────┐  ┌──────────────┐  ┌────────────────┐ │
│  │ MainPage   │  │ ScannerPage  │  │ProductDetails  │ │
│  │ (XAML/C#)  │  │ (C#)         │  │Page (C#)       │ │
│  └────────────┘  └──────────────┘  └────────────────┘ │
│         │                │                   │         │
└─────────┼────────────────┼───────────────────┼─────────┘
          │                │                   │
          └────────────────┴───────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────┐
│                     SERVICIOS                           │
│                          │                              │
│     ┌────────────────────▼───────────────────┐         │
│     │      DatabaseService                   │         │
│     │      (CRUD Operations)                 │         │
│     └────────────────────┬───────────────────┘         │
│                          │                              │
│     ┌────────────────────▼───────────────────┐         │
│     │      ZXing.Net.Maui                    │         │
│     │      (Barcode Scanning)                │         │
│     └────────────────────────────────────────┘         │
└─────────────────────────────────────────────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────┐
│                      MODELOS                            │
│                          │                              │
│     ┌────────────────────▼───────────────────┐         │
│     │         Product                        │         │
│     │         (Entity Model)                 │         │
│     └────────────────────────────────────────┘         │
└─────────────────────────────────────────────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────┐
│                   PERSISTENCIA                          │
│                          │                              │
│     ┌────────────────────▼───────────────────┐         │
│     │      SQLite Database                   │         │
│     │      (Local Storage)                   │         │
│     └────────────────────────────────────────┘         │
│                                                         │
│     ┌────────────────────────────────────────┐         │
│     │      File System                       │         │
│     │      (Photo Storage)                   │         │
│     └────────────────────────────────────────┘         │
└─────────────────────────────────────────────────────────┘
```

## Interacción Usuario - Sistema

```
┌──────────┐
│ Usuario  │
└────┬─────┘
     │
     │ 1. Inicia App
     ▼
┌──────────────────────┐
│   MainPage           │
│   - Lista productos  │◄───┐
│   - Botón escanear   │    │
└──────────────────────┘    │
     │                      │
     │ 2. Toca "Escanear"   │
     ▼                      │
┌──────────────────────┐    │
│   ScannerPage        │    │
│   - Cámara activa    │    │
│   - Detecta código   │    │
└──────────────────────┘    │
     │                      │
     │ 3. Código detectado  │
     ▼                      │
┌──────────────────────┐    │
│ DatabaseService      │    │
│ - Busca producto     │    │
└──────────────────────┘    │
     │                      │
     │ 4. Resultado         │
     ▼                      │
┌──────────────────────┐    │
│ ProductDetailsPage   │    │
│ - Muestra/Edita      │    │
│ - Captura foto       │    │
└──────────────────────┘    │
     │                      │
     │ 5. Guarda cambios    │
     ▼                      │
┌──────────────────────┐    │
│ DatabaseService      │    │
│ - Guarda en SQLite   │    │
└──────────────────────┘    │
     │                      │
     │ 6. Éxito             │
     └──────────────────────┘
```

## Estados de la Aplicación

```
┌─────────────────────────────────────────┐
│          Estado: INICIO                 │
│                                         │
│  - Base de datos: No inicializada      │
│  - Productos: Lista vacía              │
│  - UI: Cargando                        │
└─────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│       Estado: LISTA PRODUCTOS           │
│                                         │
│  - Base de datos: Inicializada         │
│  - Productos: Cargados desde BD        │
│  - UI: Mostrando lista                 │
│                                         │
│  Acciones disponibles:                 │
│  → Escanear código                     │
│  → Ver detalles de producto            │
└─────────────────────────────────────────┘
        │                    │
        │ Escanear           │ Ver detalles
        ▼                    ▼
┌──────────────┐    ┌──────────────┐
│   ESCANEANDO │    │   EDITANDO   │
│              │    │   PRODUCTO   │
│ - Cámara ON  │    │              │
│ - Detectando │    │ - Formulario │
│              │    │ - Validando  │
└──────────────┘    └──────────────┘
        │                    │
        │ Detectado          │ Guardado
        ▼                    ▼
┌─────────────────────────────────────────┐
│      Estado: GUARDANDO                  │
│                                         │
│  - Validando datos                     │
│  - Escribiendo a BD                    │
│  - Guardando foto                      │
└─────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│       Estado: CONFIRMACIÓN              │
│                                         │
│  - Muestra mensaje de éxito            │
│  - Regresa a lista de productos        │
└─────────────────────────────────────────┘
```

## Ciclo de Vida de una Foto

```
1. Usuario toca "Capturar Foto"
        │
        ▼
2. MediaPicker.CapturePhotoAsync()
        │
        ▼
3. Sistema abre cámara nativa
        │
        ▼
4. Usuario toma foto
        │
        ▼
5. Foto devuelta como FileResult
        │
        ▼
6. Generar nombre único: product_{GUID}.jpg
        │
        ▼
7. Copiar a FileSystem.AppDataDirectory
        │
        ▼
8. Guardar ruta en Product.PhotoPath
        │
        ▼
9. DatabaseService.SaveProductAsync()
        │
        ▼
10. Ruta guardada en SQLite
        │
        ▼
11. Al cargar producto:
    ImageSource.FromFile(PhotoPath)
```

## Manejo de Errores

```
┌────────────────────────────────────┐
│   Punto de Error Potencial         │
└────────────────────────────────────┘
                │
                ▼
        ┌──────────────┐
        │ try { ... }  │
        └──────────────┘
                │
       ┌────────┴────────┐
       │                 │
   Éxito             Exception
       │                 │
       ▼                 ▼
   Continuar    ┌──────────────────┐
                │ catch (Exception)│
                └──────────────────┘
                        │
                        ▼
                ┌──────────────────┐
                │ DisplayAlert     │
                │ - Título: "Error"│
                │ - Mensaje        │
                │ - Botón: "OK"    │
                └──────────────────┘
                        │
                        ▼
                Usuario informado
                Puede corregir o cancelar
```

---

Este documento proporciona una vista visual completa del flujo de la aplicación,
ayudando a entender cómo interactúan los diferentes componentes y cómo fluyen
los datos a través del sistema.
