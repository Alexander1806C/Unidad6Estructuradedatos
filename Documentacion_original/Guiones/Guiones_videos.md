# Tarea 6: Guiones de los dos videos

UNIDAD 6  /  MÉTODOS DE BÚSQUEDA

Explicación conceptual y demostración del programa en consola

| Datos académicos | Información |
| --- | --- |
| Alumno | [Nombre completo del alumno] |
| Matrícula y grupo | [Matrícula] / [Grupo] |
| Institución | [Institución] |
| Asignatura | [Asignatura] |
| Docente | [Docente] |
| Fecha | 19 de noviembre de 2026 |

Competencia: conoce, comprende y aplica algoritmos de búsqueda para resolver problemas del entorno.

## Antes de grabar

Prepara dos videos, uno por tarea. Captura la pantalla y mantén tu cámara visible en la esquina inferior derecha. Publica ambos como públicos en el mismo canal del alumno usado en las demás unidades. Este material contiene guiones, no grabaciones ni enlaces publicados.

Completa los datos de portada; abre la solución y ejecuta demo y pruebas. Ajusta el tamaño de texto para que sea legible y coloca la cámara sin cubrir las tablas ni la salida del programa. Explica con tus palabras y prueba audio antes de grabar.

## Video 6.1: conceptos y comparación (7-9 min)

### 0:00-1:00 | Problema

“Buscar permite localizar un registro por su clave. Compararé tres técnicas considerando preparación, consultas y memoria.” Muestra el objetivo y explica n, m y α.

### 1:00-2:30 | Secuencial y binaria

Presenta el cuadro. Explica una visita frente a reducir un intervalo; señala el orden previo y acceso por índice de binaria. Diferencia mejor caso y peor caso.

### 2:30-4:00 | Función hash

Usa m=11 y claves 7,18,29. Calcula sus residuos y explica por qué todavía hay que comparar claves. Distingue duplicado de colisión.

### 4:00-6:00 | Colisiones

Explica encadenamiento, sondeo lineal, cuadrático y doble hashing con las fórmulas del PDF. Menciona agrupamiento y por qué una celda borrada puede necesitar una marca.

### 6:00-7:30 | Elección y cierre

“Hash ofrece costo constante esperado bajo condiciones, pero puede recorrer n nodos. Binaria necesita orden. Para una consulta pequeña puede bastar secuencial.” Cita las fuentes y comenta el costo de preparar datos.

## Video 6.2: implementación y evidencia (8-10 min)

### 0:00-1:30 | Demostración

Abre el menú y selecciona 1. Busca 18 y 40 en el ejemplo. Explica que la posición siempre corresponde al arreglo original. La binaria devuelve 2 aunque el índice en la copia ordenada es 6.

### 1:30-3:00 | Código

Muestra Secuencial, Binaria e Indice; luego Nodo y Buscar de TablaHash. Señala la normalización del módulo y el recorrido de la cadena. Explica que no se usa Dictionary para buscar.

### 3:00-4:00 | Contadores

“Comparaciones cuenta igualdad y menor que entre claves. Iteraciones cuenta visitas. Binaria puede realizar dos comparaciones por visita; hash vacío tiene cero visitas, pero calcula h una vez.”

### 4:00-5:30 | Experimento

Explica tamaños, tres semillas, pares únicos, 500 aciertos y 500 fallos impares. Ejecuta experimento con carpeta nueva y muestra el CSV. No presentes los resultados entregados como medidos en tu equipo si no lo ejecutaste allí.

### 5:30-7:00 | Tablas y gráficas

Muestra éxitos y fallos separados, la escala logarítmica y el efecto de la carga. Aclara que se midieron conteos, no tiempo de CPU.

### 7:00-8:30 | Caso adverso y pruebas

Explica los 200 múltiplos de 211; un fallo hace 200 visitas. Ejecuta pruebas y muestra cero fallos. Resume cuándo elegir cada método.

### 8:30-9:30 | Entrega

Muestra el enlace real de tu repositorio con fuente, README y evidencia. Añade los enlaces públicos a los PDF después de publicar.

## Lista de entrega

| Tarea | Material preparado | Pendiente |
| --- | --- | --- |
| 6.1 | Cuadro, análisis, referencias PDF y texto editable. | Datos personales y video público. |
| 6.2 | Código, solución, pruebas, CSV, gráficas y reporte. | Publicar repositorio, grabar video y añadir enlaces. |

Repositorio: [Pendiente: enlace real de GitHub]

Video 6.1: [Pendiente: video público 6.1]

Video 6.2: [Pendiente: video público 6.2]

Comprueba que ambos videos se pueden abrir sin iniciar sesión. Conserva el mismo canal del alumno. Los espacios pendientes deben sustituirse por tus enlaces reales; ningún documento afirma que ya se publicaron.
