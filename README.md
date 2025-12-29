# QPV Control de Mercancías

Una aplicación móvil multiplataforma desarrollada con .NET MAUI para el control de inventario mediante escaneo de códigos de barras.

## Características

- ✅ Escaneo de códigos de barras usando la cámara del dispositivo
- ✅ Base de datos SQLite local para almacenamiento de productos
- ✅ Captura de fotos de productos
- ✅ Gestión completa de productos:
  - Código de barras
  - Descripción
  - Unidad de medida
  - Costo
  - Precio de venta
  - Stock actual
- ✅ Interfaz de usuario intuitiva en español
- ✅ Soporte para Android (iOS disponible al compilar en macOS)

## Tecnologías Utilizadas

- **.NET MAUI 10.0** - Framework multiplataforma
- **C#** - Lenguaje de programación
- **SQLite** - Base de datos local
- **ZXing.Net.Maui** - Librería para escaneo de códigos de barras
- **sqlite-net-pcl** - ORM para SQLite

## Requisitos

- .NET SDK 10.0 o superior
- Visual Studio 2022 o Visual Studio Code
- Para Android: Android SDK
- Para iOS: macOS con Xcode

## Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/BtoBtoAR/QPV-Control-Mercancias.git
cd QPV-Control-Mercancias
```

### 2. Instalar el workload de .NET MAUI

```bash
dotnet workload install maui-android
# Para iOS (solo en macOS):
# dotnet workload install maui-ios
```

### 3. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 4. Compilar el proyecto

```bash
dotnet build
```

## Ejecución

### Android

```bash
dotnet build -t:Run -f net10.0-android
```

### iOS (solo en macOS)

```bash
dotnet build -t:Run -f net10.0-ios
```

## Estructura del Proyecto

```
QPVControlMercancias/
├── Models/
│   └── Product.cs              # Modelo de datos de producto
├── Services/
│   └── DatabaseService.cs      # Servicio de base de datos SQLite
├── Pages/
│   ├── ScannerPage.cs         # Página de escaneo de códigos de barras
│   └── ProductDetailsPage.cs  # Página de detalles/edición de producto
├── MainPage.xaml              # Página principal con lista de productos
├── App.xaml.cs                # Configuración de la aplicación
└── MauiProgram.cs             # Configuración de servicios e inyección de dependencias
```

## Uso de la Aplicación

1. **Escanear un producto**: 
   - Presionar el botón "Escanear Código de Barras"
   - Apuntar la cámara al código de barras
   - La aplicación detectará automáticamente el código

2. **Producto existente**:
   - Se mostrará la información del producto
   - Se puede actualizar el stock y otros datos
   - Presionar "Guardar Producto" para confirmar cambios

3. **Producto nuevo**:
   - Se mostrará un formulario vacío con el código de barras
   - Capturar foto del producto (opcional)
   - Completar todos los campos requeridos
   - Presionar "Guardar Producto" para crear el producto

4. **Ver lista de productos**:
   - La página principal muestra todos los productos registrados
   - Tocar un producto para ver sus detalles

## Permisos Requeridos

### Android
- Cámara
- Almacenamiento (lectura/escritura)

### iOS
- Cámara
- Galería de fotos

## Paquetes NuGet

- `Microsoft.Maui.Controls` - Framework MAUI
- `sqlite-net-pcl` - ORM SQLite
- `SQLitePCLRaw.bundle_green` - SQLite para múltiples plataformas
- `ZXing.Net.Maui` - Escaneo de códigos de barras
- `ZXing.Net.Maui.Controls` - Controles de UI para ZXing

## Licencia

Este proyecto está bajo una licencia privada. Todos los derechos reservados.

## Autor

BtoBtoAR

## Notas Adicionales

- La base de datos SQLite se crea automáticamente en el primer uso
- Las fotos de productos se almacenan en el directorio de datos de la aplicación
- La aplicación funciona completamente offline

## Soporte

Para reportar problemas o solicitar características, por favor abra un issue en el repositorio de GitHub.
