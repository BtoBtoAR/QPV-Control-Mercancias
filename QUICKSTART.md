# Guía de Inicio Rápido - QPV Control Mercancias

## Requisitos Previos

- Windows 10/11, macOS, o Linux
- .NET SDK 10.0 o superior
- Visual Studio 2022 o Visual Studio Code
- Para desarrollo Android: Android SDK
- Para desarrollo iOS: macOS con Xcode (opcional)

## Configuración Inicial

### 1. Verificar .NET SDK

```bash
dotnet --version
# Debe mostrar 10.0.x o superior
```

Si necesitas instalar .NET SDK:
- Windows/macOS/Linux: https://dotnet.microsoft.com/download

### 2. Instalar Workload de .NET MAUI

```bash
# Para Android
dotnet workload install maui-android

# Para iOS (solo en macOS)
dotnet workload install maui-ios
```

Verificar instalación:
```bash
dotnet workload list
```

### 3. Clonar el Repositorio

```bash
git clone https://github.com/BtoBtoAR/QPV-Control-Mercancias.git
cd QPV-Control-Mercancias
```

### 4. Restaurar Dependencias

```bash
dotnet restore
```

### 5. Compilar el Proyecto

```bash
# Compilación Debug
dotnet build

# Compilación Release
dotnet build -c Release
```

## Ejecutar la Aplicación

### Opción A: Android (Emulador o Dispositivo)

#### Configurar Emulador Android

1. Abrir Android Studio o usar `sdkmanager`
2. Crear un AVD (Android Virtual Device):
   - API Level 21 o superior
   - Con Google Play Services
   - Con cámara habilitada

#### Ejecutar en Emulador

```bash
# Iniciar emulador (si no está corriendo)
emulator -avd [nombre_del_avd]

# En otra terminal, ejecutar la app
dotnet build -t:Run -f net10.0-android
```

#### Ejecutar en Dispositivo Físico

1. Habilitar "Opciones de Desarrollador" en el dispositivo
2. Activar "Depuración USB"
3. Conectar dispositivo vía USB
4. Verificar conexión: `adb devices`
5. Ejecutar:

```bash
dotnet build -t:Run -f net10.0-android
```

### Opción B: iOS (solo macOS)

#### Requisitos
- macOS 12.3 o superior
- Xcode 14 o superior
- Simulador iOS o dispositivo físico

#### Ejecutar en Simulador

```bash
dotnet build -t:Run -f net10.0-ios
```

#### Ejecutar en Dispositivo

1. Abrir proyecto en Xcode
2. Configurar signing certificates
3. Seleccionar dispositivo
4. Build & Run desde Xcode

## Desarrollo con Visual Studio 2022

### Windows

1. Abrir `QPVControlMercancias.csproj`
2. Seleccionar target framework: `net10.0-android`
3. Seleccionar emulador o dispositivo
4. Presionar F5 para ejecutar

### macOS

1. Abrir `QPVControlMercancias.csproj`
2. Seleccionar target: Android o iOS
3. Seleccionar dispositivo/emulador
4. Presionar ▶️ para ejecutar

## Desarrollo con Visual Studio Code

### Extensiones Requeridas

```bash
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
```

### Tareas Comunes

Crear archivo `.vscode/tasks.json`:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/QPVControlMercancias.csproj"
            ],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "run-android",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "-t:Run",
                "-f",
                "net10.0-android",
                "${workspaceFolder}/QPVControlMercancias.csproj"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}
```

## Estructura del Código

```
QPVControlMercancias/
├── Models/
│   └── Product.cs              # Modelo de producto
├── Services/
│   └── DatabaseService.cs      # Servicio de BD SQLite
├── Pages/
│   ├── ScannerPage.cs         # Escáner de códigos
│   └── ProductDetailsPage.cs  # Detalles de producto
├── Resources/
│   ├── AppIcon/               # Iconos de app
│   ├── Fonts/                 # Fuentes
│   ├── Images/                # Imágenes
│   └── Styles/                # Estilos XAML
├── Platforms/
│   ├── Android/               # Código Android
│   └── iOS/                   # Código iOS
├── App.xaml[.cs]              # Aplicación principal
├── MainPage.xaml[.cs]         # Página principal
└── MauiProgram.cs             # Configuración
```

## Modificar la Aplicación

### Agregar un nuevo campo al producto

1. Editar `Models/Product.cs`:

```csharp
public string? NewField { get; set; }
```

2. Agregar campo en `ProductDetailsPage.cs`:

```csharp
_newFieldEntry = new Entry { Placeholder = "Nuevo campo" };
stackLayout.Add(_newFieldEntry);
```

3. Guardar en `OnSaveClicked`:

```csharp
product.NewField = _newFieldEntry.Text;
```

### Cambiar colores de la UI

Editar `Resources/Styles/Colors.xaml`:

```xml
<Color x:Key="Primary">#512BD4</Color>
<Color x:Key="Secondary">#DFD8F7</Color>
```

### Cambiar el icono de la app

Reemplazar `Resources/AppIcon/appicon.svg` con tu icono en formato SVG.

## Debugging

### Logs en la Consola

```csharp
Debug.WriteLine("Mensaje de debug");
Console.WriteLine("Mensaje en consola");
```

### Ver logs en Android

```bash
adb logcat | grep "QPVControlMercancias"
```

### Breakpoints

- Visual Studio: Click en el margen izquierdo del código
- VS Code: Click en el margen o presiona F9

## Problemas Comunes

### Error: No se encuentra el SDK de Android

**Solución:**
```bash
# Windows
set ANDROID_SDK_ROOT=C:\Android\sdk

# macOS/Linux
export ANDROID_SDK_ROOT=$HOME/Android/Sdk
```

### Error: Workload no instalado

**Solución:**
```bash
dotnet workload install maui-android
```

### Error: Emulador no inicia

**Solución:**
1. Verificar HAXM/Hyper-V está habilitado
2. Verificar virtualización en BIOS
3. Usar emulador más reciente

### Error: Permisos de cámara

**Solución:**
En Android, los permisos se solicitan automáticamente en runtime. Asegúrate de que `AndroidManifest.xml` contiene:
```xml
<uses-permission android:name="android.permission.CAMERA" />
```

## Testing

### Test Manual

1. Escanear código de barras nuevo
2. Capturar foto
3. Llenar formulario
4. Guardar producto
5. Verificar que aparece en la lista
6. Seleccionar producto de la lista
7. Modificar stock
8. Guardar cambios

### Verificar Base de Datos

Para Android:
```bash
adb shell
cd /data/data/com.companyname.qpvcontrolmercancias/files
cat products.db
```

## Publicación

### Android APK

```bash
dotnet publish -f net10.0-android -c Release
```

El APK se genera en: `bin/Release/net10.0-android/publish/`

### Android App Bundle (AAB) para Google Play

```bash
dotnet publish -f net10.0-android -c Release /p:AndroidPackageFormat=aab
```

### iOS App Store

Requiere certificados y provisioning profiles de Apple Developer.

1. Configurar en Xcode
2. Archive → Distribute App
3. Seguir el wizard de Xcode

## Recursos Adicionales

- [Documentación .NET MAUI](https://learn.microsoft.com/dotnet/maui/)
- [ZXing.Net.Maui GitHub](https://github.com/Redth/ZXing.Net.Maui)
- [SQLite-net Documentation](https://github.com/praeclarum/sqlite-net)
- [MAUI Community Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/maui/)

## Soporte

¿Problemas? ¿Preguntas?

1. Revisar [Issues](https://github.com/BtoBtoAR/QPV-Control-Mercancias/issues)
2. Crear un nuevo issue si es necesario
3. Incluir:
   - Sistema operativo
   - Versión de .NET SDK
   - Pasos para reproducir
   - Logs relevantes

## Licencia

Este proyecto es privado. Todos los derechos reservados.

---

**¡Feliz desarrollo! 🚀**
