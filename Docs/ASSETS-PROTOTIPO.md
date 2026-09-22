# Assets do protótipo

Seleção verificada em 21/09/2026. Direção: low poly sério, proporções humanas,
vestuário civil, materiais gastos e cores desbotadas. Evitar cabeças enormes,
expressões cômicas, brilho plástico e paletas de brinquedo. Referências como
The Walking Dead orientam ritmo e atmosfera; não copiar personagens ou marcas.

## Modelo integrado para o marco 2A

**Male_LongSleeve, de Quaternius — Animated Men Pack.** Civil de manga comprida,
1,98 MB, FBX com esqueleto e ciclos de locomoção. O autor oferece o conjunto sob
[CC0](https://quaternius.com/packs/animatedmen.html), com uso comercial permitido.
[Preview oficial](https://quaternius.com/assets/images/fullres/animatedmen.jpg).

Foi selecionado apenas um humano para os testes entre dois jogadores. Continua
estilizado e provisório: não é uma aprovação de toda a identidade visual do pacote.
O projeto substitui cores por tons de tecido, couro e pele, preservando os bytes
originais. Fonte, licença e checksum estão em
`Assets/ThirdParty/QuaterniusAnimatedMen/NOTICE.md`. Assets binários usam Git LFS.

A integração em `SurvivorVisual` é apenas visual: sem colisores, autoridade sobre
posição ou lógica de dano. O importador usa Generic, portanto não depende de uma
conversão Humanoid para tocar as animações do próprio arquivo. A câmera FPS não
renderiza o corpo local; os outros clientes veem o civil. Materiais URP próprios
evitam depender de shaders de outro pipeline. Idle e Walk fazem parte do recorte;
animações específicas de arma e câmera de veículos ficam para os próximos marcos.

## Candidatos reservados, ainda não importados

| Uso | Fonte primária / preview | Licença indicada | Avaliação |
|---|---|---|---|
| Humano de proporção regular | [Universal Base Characters](https://quaternius.com/packs/universalbasecharacters.html) | CC0, Standard gratuito | Alternativa anatômica para evolução; média anunciada de 13 mil triângulos, rig Humanoid, FBX/glTF. A base não resolve roupa civil; Source com projeto Unity é uma edição distinta. |
| Barril de depósito | [Poly Haven — Barrel 02](https://polyhaven.com/a/Barrel_02) | CC0 | Forma civil sóbria, cerca de 3 mil triângulos; texturas e detalhes podem exigir simplificação para combinar com low poly. |
| Objetos de sobrevivência | [Quaternius — Survival Pack](https://quaternius.com/packs/survival.html) | CC0 | Avaliar somente objetos civis pontuais, ajustando materiais. Sem aprovação automática do pacote completo. |

O [Zombie Apocalypse Kit](https://quaternius.com/packs/zombieapocalypsekit.html)
tem licença CC0 e animações, mas suas criaturas de cores vivas e caricatura não
foram selecionadas para a direção atual. Os antigos candidatos Kenney com visual
de brinquedo também não definem a direção do Ferrugem. O zombie definitivo continua pendente. Para o marco 2B, o civil existente é reaproveitado com cores dessaturadas, locomoção lenta e antecipação visual provisória de ataque.

O [Low Poly Industrial Props de maxorbie](https://maxorbie.itch.io/low-poly-industrial-props)
não foi importado: a descrição menciona CC BY 4.0 e o campo de licença indica CC0.
A divergência precisa ser esclarecida antes de uma eventual inclusão.

## Regra de inclusão

Registrar autor, URL original, licença por arquivo, data, checksum e modificações.
Conferir formato, escala, orientação, animação, materiais URP e custo de renderização
em build Windows. Arquivos externos não são prova de compatibilidade automática.
Não importar scripts de lojas ou pacotes completos sem necessidade. Usar apenas o
subconjunto necessário ao teste. Preparar colisão e destruição separadamente:
um prédio low poly não vem necessariamente pronto para quebrar.

## Apresentação provisória do marco 2B

`SurvivorVisual.CreateZombie` reutiliza o mesmo FBX CC0, sem alterar seu conteúdo.
A variação de materiais é cinza/verde desbotada e a antecipação do ataque combina
inclinação visual com uma marca no chão. Não foi incorporado outro pacote.
Revólver, barril e carga usam primitivas Unity montadas por código com assistência
de IA. São referências de escala e funcionamento; não são os assets finais.

## Referências adicionais do usuário

As cinco referências Synty e a distinção entre direção de arte e sistemas de jogo
estão em [Referências visuais e de controle FPS](REFERENCIAS-VISUAIS-E-FPS.md).
Nenhum dos pacotes pagos foi importado. A seleção atual continua gratuita/CC0 e
provisória, com geometria original para os objetos de teste.

## Decisão de produção: arte original brasileira

O usuário definiu **não comprar os pacotes Synty**. Eles permanecem referências
visuais, sem extração ou cópia de conteúdo. A produção terá arte própria e PT-BR,
conforme o [briefing do mapa brasileiro](MAPA-BRASILEIRO.md). O civil CC0 integrado
continua apenas como recurso temporário de validação; não houve pedido de remoção
imediata nem transferência de sua autoria para o projeto.

## Refinamento 0.3.0: mãos e sons originais

Mãos, dedos, punhos e manga foram montados com geometria original simples por
código, com assistência de IA. Recuo e recarga são animações procedurais. Os sons
são sintetizados em `AudioClip` em memória a partir de ruído e osciladores; não
são gravações, áudio de terceiros ou saída de serviço generativo externo. São
placeholders que exigem revisão visual e escuta. O arquivo Quaternius continua
inalterado; agachamento aplica pose de ossos na apresentação e não altera a autoria.
