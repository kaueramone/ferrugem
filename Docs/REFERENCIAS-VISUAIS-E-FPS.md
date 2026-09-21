# Referências visuais e de controle FPS

Referências indicadas pelo usuário e verificadas nas páginas oficiais em
21/09/2026. Este documento orienta os próximos ajustes; não declara que os recursos
abaixo já estão implementados nem que os pacotes foram comprados ou importados.

## Sensação de controle: POLYGON

[POLYGON — Multiplayer Shooter, na Steam](https://store.steampowered.com/app/1241100/POLYGON_Multiplayer_Shooter/)
é a referência indicada para a experiência de FPS. Sua página o descreve como um
jogo de tiro multiplayer em primeira pessoa, com combate de equipes e opções de
armamento. O Ferrugem mantém sua própria proposta: três grupos de sobreviventes,
armas e veículos civis, KOTH, logística e infectados lentos.

Vamos observar resposta do movimento, mira, leitura dos acertos e apresentação da
arma. Velocidade, aceleração, FOV, sensibilidade, recuo e tempos específicos do
POLYGON ainda não foram medidos em gameplay; qualquer valor proposto para o
Ferrugem será uma hipótese de playtest. Não há acesso nem adoção do código do jogo.

## Direção de arte: referências Synty

| Referência oficial | Aplicação proposta no Ferrugem | Limite observado |
| --- | --- | --- |
| [Woodland Apocalypse Map](https://syntystore.com/products/polygon-woodland-apocalypse-map) | Relação entre vegetação, estrada, pequena cidade e pontos civis de interesse. | É uma expansão que requer **Apocalypse Pack e Alpine Mountain Nature Biome**. A página anuncia Unity 2022.3+ com URP; isso não substitui teste no nosso Unity 6000.3. |
| [Apocalypse Wasteland](https://syntystore.com/products/polygon-apocalypse-wasteland) | Materiais remendados, sucata e assentamentos improvisados. | Selecionar a linguagem de desgaste; mutantes luminosos e exageros do conjunto não definem nossos infectados. |
| [Apocalypse Pack](https://syntystore.com/products/polygon-apocalypse-pack) | Cenário urbano deteriorado e peças modulares para ocupação por sobreviventes. | A cena demonstrativa não será copiada como mapa do jogo. |
| [City Zombies Pack](https://syntystore.com/products/polygon-city-zombies-pack) | Diversidade de roupas civis e leitura visual dos infectados. | A página informa personagens preparados para Mecanim, **sem animações incluídas**. |
| [Battle Royale Pack](https://syntystore.com/products/polygon-battle-royale-pack) | Escala de objetos e organização visual de áreas abertas e construídas. | O próprio pacote tem tema militar; isso não altera a regra do Ferrugem de armamento e veículos predominantemente civis. |

Os pacotes listados são conteúdo de arte e cenas de demonstração. Não são uma
implementação do controlador FPS, dano, zombies, rede, KOTH ou destruição do
Ferrugem. Não foi demonstrado que estes pacotes sejam os usados pelo jogo POLYGON;
a coincidência de nomes não estabelece essa relação.

## Caminho sem compra de assets agora

Sem orçamento de compra definido, seguimos com o civil CC0 já integrado, recursos
gratuitos com procedência verificada e geometria original para o bloco de teste.
A referência orienta proporções, paleta, densidade de detalhes e atmosfera. Layout,
silhuetas e composição serão próprios; não serão usados arquivos extraídos dos
produtos nem réplicas exatas dos seus modelos. Uma futura compra exigirá verificar
conteúdo necessário, dependências, compatibilidade e licença daquele produto.

A [licença de compra única da Synty](https://syntystore.com/pages/one-time-purchase-licence)
consultada contém restrições específicas ao uso dos assets em datasets e programas
de IA generativa, geração de modelos 3D com esses programas e certos materiais
relacionados a produtos/programas de IA generativa. Isso exige conferir o uso
concreto de um asset antes de incorporá-lo nesse tipo de fluxo. O texto consultado
não fundamenta a conclusão genérica de que qualquer jogo cujo código teve
assistência de IA seja proibido. Nenhum asset pago foi enviado a ferramenta
generativa neste trabalho.

## Próximo recorte: refinar o FPS antes de ampliar a arte

Após validar o combate atual do marco 2B, o próximo recorte proposto é pequeno e
mensurável. Não é uma promessa de recursos já presentes:

1. **Movimento:** medir início/parada, aceleração e corrida; testar agachamento,
   rampas e escadas mantendo cliente e servidor consistentes. O controlador atual
   ainda é plano e usa colisões simples.
2. **Mira:** alinhar a mira de ferro em primeira pessoa, definir transição de FOV
   e sensibilidade ao mirar, sem criar câmera de terceira pessoa a pé.
3. **Arma:** ajustar recuo visual, balanço ao andar, retorno à mira e animação de
   recarga. Separar o efeito de apresentação da decisão de acerto no servidor.
4. **Áudio:** integrar disparo, recarga, passos e sinais dos infectados com origem
   e licença registradas; regular alcance e volume para comunicar o perigo.
5. **Rede:** repetir os testes autoritativos com latência e perda simuladas;
   conferir acerto, consumo de munição e morte em ambos os clientes antes de
   ampliar mapa, quantidade de personagens ou detalhe artístico.

O aceite desse recorte deverá comparar controles e legibilidade no campo pequeno,
com observação humana e registros de rede. O alvo de 100 jogadores continua
separado e requer testes de carga próprios.

## Aplicação ao mapa e à destruição

A atmosfera de cidade tomada pela vegetação cabe no Ferrugem, mas o mapa será
desenhado para três bases e uma zona central com mercado, posto de combustível,
delegacia e depósitos. Compararemos tempo de deslocamento, cobertura e acesso a
posições elevadas para cada grupo. A distribuição de uma cena demonstrativa não
é evidência de equilíbrio para KOTH.

O primeiro conjunto visual deve ser mínimo: um veículo civil, um módulo de
edificação, uma árvore, um infectado e uma arma civil. Cada construção destinada
à destruição precisará de estados íntegro, danificado e destruído, com colisão e
replicação correspondentes. Comprar uma malha não implementa essa lógica.

A simulação de entidades e suas regras permanecem separadas da apresentação. Isso
permite substituir os visuais provisórios por arte própria ou licenciada depois,
sem condicionar o funcionamento do protótipo à compra de um pacote.
