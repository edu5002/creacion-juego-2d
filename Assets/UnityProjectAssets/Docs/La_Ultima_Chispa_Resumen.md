# La Última Chispa — Resumen de Propuesta de Juego

**Motor:** Unity
**Género:** Plataformas / Acción 2D
**Estilo visual:** Pixel art, paleta cálida (dorado/blanco) para la luz vs. fría (morado/gris) para la corrupción

---

## Premisa

Existía un reino de magia protegido por un manantial de luz ancestral. El manantial se fracturó, y la corrupción se extiende por el mundo, apagando la magia y transformando criaturas y guardianes en versiones oscuras y hostiles de sí mismas. El jugador controla la última chispa de esa luz original, que debe atravesar tres regiones corrompidas hasta llegar a la fuente de la fractura.

Esta premisa funciona como justificación narrativa transversal: explica la existencia de enemigos en cada nivel, el propósito del jefe final, y por qué el sistema de vida se representa como "intensidad de luz" en vez de una barra genérica.

---

## Protagonista: Lumen

**Qué es:** un fragmento vivo del manantial de luz, no humanoide — forma de gota/llama con un núcleo brillante, sin rostro ni extremidades. No pelea por heroísmo: es literalmente parte de lo que se está muriendo.

**Personalidad:** transmitida por animación, no por diálogo — pequeño pero terco. Flota y pulsa en reposo; se apaga visualmente al recibir daño en vez de "reaccionar" con gestos.

**Sprite sheet:** generado en Gemini, grid de 4×4 (64×64 aprox. por celda), fondo transparente. Confirmado visualmente correcto; pendiente verificar que las 16 celdas tengan tamaño y centrado uniformes antes de importar a `AnimatedSprite2D`.

| Fila | Animación | Frames | Función |
|---|---|---|---|
| 1 | Idle | 4 | Reposo, flotando/pulsando |
| 2 | Dash | 4 | Movimiento rápido con estela, mirando a la derecha |
| 3 | Hurt | 4 | Destello blanco al impacto → tono corrupto apagado |
| 4 | Charge | 4 | Energía creciente con anillos/partículas orbitando |

**Habilidades (progresión por nivel):**

| Habilidad | Función | Se desbloquea en |
|---|---|---|
| Movimiento base + salto | Exploración estándar | Nivel 1 |
| Dash de luz | Ataque/movimiento direccional | Nivel 1 |
| Carga de luz | Golpe cargado, más alcance/daño | Nivel 2 |
| Dash aéreo | Segundo dash en el aire | Nivel 3 |

---

## Mundo y niveles

| Nivel | Región | Tema visual | Progresión |
|---|---|---|---|
| 1 | Bosque Encantado | Verdes/dorados apagándose a grises (corrupción leve, no total) | Tutorial de movimiento + dash; termina en una entrada de cueva con resplandor púrpura/azulado que anticipa el Nivel 2 |
| 2 | Cavernas de Cristal | Azules/púrpuras, cristales rotos (retoma la corrupción leve del final del Nivel 1 y avanza a media) | Nuevos enemigos/obstáculos, dificultad media |
| 3 | Santuario del Manantial | Dorado brillante vs. negro/violeta corrupto (corrupción media-alta a total) | Combina todas las mecánicas, culmina en el jefe |

**Nota de continuidad:** la corrupción avanza como una sola curva a lo largo de los 3 niveles, no se reinicia entre uno y otro. El final de cada nivel puede insinuar el siguiente con un adelanto de color/silueta (ej. el resplandor de las Cavernas ya visible en la entrada al final del Bosque).

---

## Lore

Lore deliberadamente breve — el peso narrativo lo lleva el ambiente de cada nivel, no el texto. Todo el texto "obligatorio" del juego cabe en unas pocas líneas; el resto es opcional.

**Apertura (antes del Nivel 1):**

> El manantial que sostenía la magia del mundo se fracturó.
> De su luz, un fragmento escapó.
> Todo lo demás... empezó a corromperse.

**Ecos (opcionales, en altares/checkpoints — no bloquean el avance):**

Reutilizan el mismo objeto que ya se necesita como punto de guardado, sin costo de diseño extra.

| Nivel | Altar | Eco |
|---|---|---|
| Bosque Encantado | 1 (medio) | "Aquí crecían flores que nunca se marchitaban. Ahora solo caen." |
| Bosque Encantado | 2 (final) | "Los árboles aún recuerdan la luz. Por eso resisten." |
| Cavernas de Cristal | 1 (medio) | "Aquí cantaban los cristales. Ahora solo gritan." |
| Cavernas de Cristal | 2 (final) | "Lo que antes brillaba, ahora corta." |
| Santuario del Manantial | 1 (medio) | "Este era el corazón del mundo. Ahora es una herida." |
| Santuario del Manantial | 2 (antes del jefe) | "Lo que queda de mí... me espera." |

El último eco es intencional como presagio sutil de que el jefe final es la otra mitad de Lumen, sin revelarlo todavía.

**Cierre (después del jefe):**

> El eco se apagó. La luz, no.
> El manantial no volvió a ser lo que fue.
> Pero volvió a brillar.

---

## Jefe final: Eco Corrupto

**Qué es:** la otra mitad de lo que Lumen fue alguna vez — la parte del manantial que no logró escapar y se corrompió con el tiempo. No es malvado conscientemente: es dolor y fragmentación convertidos en forma. El clímax no es "vencer al malo", es una confrontación con lo que Lumen pudo haber sido.

**Apariencia:** espejo oscuro y roto de la silueta de Lumen — misma forma de gota/llama, pero agrietada, más grande, con fragmentos flotando y "goteando" corrupción en vez de emitir luz.

**Fases de ataque:**

| Fase | Patrón de ataque | Habilidad que exige |
|---|---|---|
| 1 | Ráfagas de fragmentos corruptos a distancia | Dash básico para esquivar/cerrar distancia |
| 2 | Charcos de corrupción en el suelo que se expanden | Dash cargado para cruzar o destruir zonas |
| 3 | Imita el dash de Lumen, lanzándose como un reflejo | Dash aéreo para esquivar en el aire |

---

## Cómo cubre los criterios de evaluación

| Criterio | Cómo se resuelve en esta propuesta |
|---|---|
| Jefe final | Eco Corrupto, 3 fases con patrones únicos ligados a las habilidades del jugador |
| Sistema de vidas y daño | "Luz" en vez de HP genérico; el sprite se apaga visualmente al recibir daño |
| Pantalla de Game Over | Pendiente de diseño (siguiente etapa) |
| Sistema de continues | Propuesta: "chispas de reserva" recolectables, coherente con la narrativa |
| Múltiples niveles | 3 niveles temáticos con dificultad progresiva + jefe final |
| Guardar/cargar progreso | Altares/santuarios como checkpoints, cada uno con un eco narrativo opcional |
| Pulido y UX | Pendiente (fase de implementación) |
| Creatividad y originalidad | Protagonista no humanoide, vida = luz, jefe como reflejo corrupto del jugador |
| Optimización | Pendiente (fase de implementación) |
| Historia | Luz vs. corrupción, jefe como contraparte del protagonista — coherente en todo el arco |

---

## Estado de assets generados

- Lumen — hoja 1 (Idle/Dash/Hurt/Charge): lista, transparencia real corregida
- Lumen — hoja 2 (Move/Jump/Fall/Land): lista, transparencia real corregida
- Bosque Encantado — fondo, capa media y primer plano: listos, transparencia real corregida
- Bosque Encantado (versión corrupta), Cavernas de Cristal, Santuario del Manantial: **pendientes**

## Pendiente por definir

- Versión corrupta del Bosque Encantado (final del Nivel 1)
- Fondos de Nivel 2 (Cavernas de Cristal) y Nivel 3 (Santuario del Manantial)
- Nombre y diseño de enemigos regulares de cada nivel
- Arte del jefe final (Eco Corrupto) — el concepto ya está escrito, falta el sprite
- Diseño de pantalla de Game Over y flujo de continues
- Detalles de UI/UX (menús, HUD)
- Estructura técnica en Unity (escenas, managers, sistema de guardado)
