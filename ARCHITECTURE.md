# QPV Control Mercancias - Documentación Técnica

## Arquitectura de la Aplicación

### Vista General

Esta aplicación móvil multiplataforma está construida usando el patrón MVVM (Model-View-ViewModel) simplificado con inyección de dependencias. La arquitectura se divide en las siguientes capas:

```
QPVControlMercancias/
├── Models/              # Modelos de datos
├── Services/            # Servicios de negocio y datos
├── Pages/               # Vistas y lógica de presentación
├── Resources/           # Recursos de la aplicación
└── Platforms/           # Código específico de plataforma
```

## Componentes Principales

### 1. Modelo de Datos (Models/Product.cs)

**Clase `Product`**
- Representa un producto en la base de datos
- Atributos SQLite para mapeo ORM
- Campos:
  - `Id`: Clave primaria autoincremental
  - `Barcode`: Código de barras (indexado, único)
  - `Description`: Descripción del producto
  - `UnitOfMeasure`: Unidad de medida (pza, kg, lt, etc.)
  - `Cost`: Costo del producto
  - `SellingPrice`: Precio de venta
  - `Stock`: Cantidad en inventario
  - `PhotoPath`: Ruta de la foto del producto
  - `CreatedAt`, `UpdatedAt`: Timestamps

### 2. Capa de Servicios

#### DatabaseService (Services/DatabaseService.cs)

Servicio singleton que maneja todas las operaciones de base de datos SQLite.

**Métodos principales:**
- `GetProductsAsync()`: Obtiene todos los productos
- `GetProductByBarcodeAsync(string barcode)`: Busca por código de barras
- `SaveProductAsync(Product product)`: Inserta o actualiza un producto
- `DeleteProductAsync(Product product)`: Elimina un producto
- `GetProductCountAsync()`: Cuenta total de productos

**Características:**
- Inicialización lazy de la conexión a BD
- Operaciones asíncronas
- Thread-safe con manejo adecuado de la inicialización
- Base de datos almacenada en `FileSystem.AppDataDirectory/products.db`

### 3. Páginas (Views)

#### MainPage (MainPage.xaml / MainPage.xaml.cs)

Página principal de la aplicación.

**Funcionalidad:**
- Lista de productos en una CollectionView
- Botón para abrir el escáner
- Estadísticas de productos
- Navegación a detalles al seleccionar un producto

**Componentes UI:**
- CollectionView con DataTemplate personalizado
- EmptyView para cuando no hay productos
- Botón de escaneo prominente
- Frame con estadísticas

#### ScannerPage (Pages/ScannerPage.cs)

Página de escaneo de códigos de barras usando ZXing.Net.Maui.

**Funcionalidad:**
- Muestra vista previa de cámara
- Detecta códigos de barras 1D y 2D automáticamente
- Feedback háptico al detectar código
- Overlay con instrucciones y marco de objetivo
- Navega automáticamente a ProductDetailsPage

**Componentes UI (generados por código):**
- `CameraBarcodeReaderView`: Control de ZXing para escaneo
- Overlay con instrucciones
- Marco visual para guiar el escaneo
- Botón de cancelar

**Manejo de eventos:**
- `OnBarcodesDetected`: Procesa códigos detectados
- `OnAppearing/OnDisappearing`: Controla el estado de detección

#### ProductDetailsPage (Pages/ProductDetailsPage.cs)

Página para ver y editar detalles de productos.

**Modos de operación:**
1. **Producto existente**: Muestra datos completos, permite edición
2. **Producto nuevo**: Formulario vacío con código de barras pre-llenado

**Funcionalidad:**
- Captura de foto del producto usando la cámara
- Validación de todos los campos
- Guardado en base de datos
- Navegación de regreso a la lista

**Componentes UI (generados por código):**
- Image para foto del producto
- Button para captura de foto
- Entries para cada campo del producto
- ScrollView para permitir scroll en pantallas pequeñas
- Button de guardar

**Validaciones:**
- Campos obligatorios: Barcode, Description, UnitOfMeasure
- Validación numérica: Cost, SellingPrice, Stock
- Mensajes de error descriptivos

### 4. Configuración de la Aplicación

#### MauiProgram.cs

Punto de entrada de la aplicación donde se configuran los servicios.

**Configuración:**
- Registro de `UseBarcodeReader()` para ZXing
- Inyección de dependencias:
  - `DatabaseService` como Singleton
  - `MainPage` como Singleton
  - `App` como Singleton
- Configuración de fuentes
- Logging en modo Debug

#### App.xaml.cs

Clase principal de la aplicación.

**Funcionalidad:**
- Recibe `MainPage` vía inyección de dependencias
- Crea NavigationPage como ventana raíz
- Usa el patrón recomendado `CreateWindow` en lugar de `MainPage` obsoleto

## Flujo de Datos

### Escaneo y Búsqueda de Producto

```
MainPage
  └─> Clic en "Escanear Código de Barras"
      └─> ScannerPage
          ├─> Detecta código de barras
          ├─> Busca en DatabaseService
          ├─> Si existe: ProductDetailsPage(product)
          └─> Si no existe: ProductDetailsPage(barcode)
```

### Guardado de Producto

```
ProductDetailsPage
  └─> Usuario completa formulario
      └─> Validación de campos
          └─> DatabaseService.SaveProductAsync()
              ├─> Nuevo: INSERT
              └─> Existente: UPDATE
                  └─> Regreso a MainPage
                      └─> Recarga lista de productos
```

## Permisos de Plataforma

### Android (AndroidManifest.xml)

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
```

### iOS (Info.plist)

```xml
<key>NSCameraUsageDescription</key>
<string>Para escanear códigos de barras y capturar fotos de productos</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>Para guardar imágenes de productos</string>
```

## Dependencias NuGet

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| Microsoft.Maui.Controls | 10.0.1 | Framework MAUI |
| sqlite-net-pcl | 1.9.172 | ORM SQLite |
| SQLitePCLRaw.bundle_green | 2.1.10 | SQLite nativo multiplataforma |
| ZXing.Net.Maui | 0.4.0 | Escaneo de códigos de barras |
| ZXing.Net.Maui.Controls | 0.4.0 | Controles UI para ZXing |

## Base de Datos

### Esquema

**Tabla: products**

| Columna | Tipo | Constraints |
|---------|------|-------------|
| Id | INTEGER | PRIMARY KEY AUTOINCREMENT |
| Barcode | TEXT | NOT NULL, INDEXED |
| Description | TEXT | NOT NULL |
| UnitOfMeasure | TEXT | NOT NULL |
| Cost | REAL | |
| SellingPrice | REAL | |
| Stock | INTEGER | |
| PhotoPath | TEXT | NULLABLE |
| CreatedAt | TEXT | |
| UpdatedAt | TEXT | |

### Ubicación

- Android: `/data/data/com.companyname.qpvcontrolmercancias/files/products.db`
- iOS: `~/Library/Application Support/products.db`

## Almacenamiento de Fotos

Las fotos de productos se guardan en:
- `FileSystem.AppDataDirectory/{GUID}.jpg`
- Nombres únicos usando GUID para evitar colisiones
- Formato JPEG para optimizar espacio

## Ciclo de Vida de Páginas

### MainPage
- `OnAppearing()`: Recarga la lista de productos desde la BD

### ScannerPage
- `OnAppearing()`: Activa la detección de códigos
- `OnDisappearing()`: Desactiva la detección

### ProductDetailsPage
- Constructor: Inicializa UI y carga datos si es producto existente
- No tiene ciclo de vida especial, es modal/navegación estándar

## Manejo de Errores

La aplicación usa DisplayAlert para mostrar errores al usuario:
- Errores de validación
- Errores de base de datos
- Errores de captura de foto
- Errores de escaneo

Todos los métodos críticos están envueltos en try-catch con mensajes descriptivos.

## Consideraciones de Rendimiento

1. **Base de Datos**: Operaciones asíncronas para no bloquear UI
2. **Imágenes**: Guardadas en disco, no en BD (mejor rendimiento)
3. **Escaneo**: Detección continua pero controlada por flag `_isDetecting`
4. **Lista de productos**: CollectionView virtualizada para listas grandes

## Extensibilidad

### Agregar nuevos campos a Product

1. Actualizar `Models/Product.cs`
2. La BD se actualizará automáticamente (SQLite manejará la migración)
3. Actualizar UI en `ProductDetailsPage.cs`

### Agregar nuevas funcionalidades

1. Crear nueva página en `Pages/`
2. Registrar servicios necesarios en `MauiProgram.cs`
3. Agregar navegación desde páginas existentes

### Soportar otros tipos de códigos

ZXing ya soporta múltiples formatos. Para habilitar solo ciertos tipos:

```csharp
Options = new BarcodeReaderOptions
{
    Formats = BarcodeFormats.QRCode | BarcodeFormats.Code128,
    // ...
}
```

## Pruebas

### Para probar en dispositivo Android:

```bash
dotnet build -t:Run -f net10.0-android
```

### Para probar en emulador:

Asegúrate de tener un emulador Android configurado y ejecutando, luego:

```bash
dotnet build -t:Run -f net10.0-android
```

## Solución de Problemas

### Problema: La cámara no funciona
- Verificar permisos en AndroidManifest.xml o Info.plist
- En emulador, verificar que tiene cámara virtual habilitada

### Problema: Base de datos no se crea
- Verificar que `FileSystem.AppDataDirectory` es accesible
- Verificar permisos de escritura en dispositivo

### Problema: Códigos de barras no se detectan
- Verificar iluminación adecuada
- Verificar que el código está en formato soportado
- Verificar que la cámara tiene enfoque adecuado

## Mejoras Futuras Sugeridas

1. **Exportación de datos**: CSV, Excel
2. **Sincronización en nube**: Azure, Firebase
3. **Búsqueda avanzada**: Por descripción, rango de precios
4. **Reportes**: Inventario bajo, productos más vendidos
5. **Categorías**: Organización por categorías de productos
6. **Múltiples fotos**: Galería de imágenes por producto
7. **Historial**: Registro de cambios de stock
8. **Modo oscuro**: Soporte para tema oscuro
9. **Idiomas**: Internacionalización (i18n)
10. **Tests unitarios**: Cobertura de servicios y lógica de negocio

## Contacto y Soporte

Para reportar bugs o solicitar funcionalidades, crear un issue en el repositorio GitHub.
