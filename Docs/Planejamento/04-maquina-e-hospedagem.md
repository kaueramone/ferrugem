# Máquina de desenvolvimento e hospedagem

## Inventário observado em 21/09/2026

Consulta local somente leitura:

- Intel Core i5-12500H, 12 núcleos e 16 processadores lógicos.
- 32 GB de RAM.
- NVIDIA GeForce RTX 4060 Laptop GPU e Intel Iris Xe.
- C: cerca de 452 GB livres; D: cerca de 409 GB livres. Tipo físico de cada unidade e VRAM dedicada não foram confirmados.
- Unity Hub, Unity Editor 6000.2.2f1 e Git encontrados nos caminhos usuais.

Avaliação: boa máquina para criar e testar o protótipo com servidor local e poucos clientes. Não há necessidade demonstrada de comprar hardware agora. Ainda não medimos desempenho ou condições térmicas sustentadas. Em notebook, testar ligado à energia e observar temperatura e throttling.

## Preparação do ambiente

1. Instalar Unity 6.3 LTS lado a lado com a versão existente; escolher patch estável compatível com os pacotes e fixá-lo. [Suporte Unity](https://unity.com/releases/unity-6/support).
2. Adicionar Windows Build Support quando necessário ao backend escolhido e Linux Dedicated Server Build Support. Para IL2CPP Windows, instalar ferramentas C++/Windows SDK indicadas na documentação. [Requisitos Unity 6.3](https://docs.unity3d.com/6000.3/Documentation/Manual/system-requirements.html), [módulo servidor](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/dedicated-server/get-started/requirements).
3. Usar IDE com C#, Git e Git LFS; Blender para modelos low poly se produzirmos assets próprios. Confirmar instalação/licença de cada ferramenta antes de automatizar.
4. Reservar 100–200 GB em SSD para Editor, Library, builds e assets. 32 GB de RAM é a recomendação de trabalho inicial; 64 GB só se medições justificarem múltiplos processos/mapas maiores. Isso é estimativa do projeto, não mínimo oficial do jogo.
5. Criar o projeto ativo preferencialmente fora da sincronização do OneDrive, por exemplo em `C:\Dev\Ferrugem`, com Git e backup próprio. Manter estes documentos aqui; não mover projetos existentes automaticamente. Essa localização evita que arquivos temporários de importação disputem sincronização.
6. Usar cliente em build além do Editor; testar inicialmente dois clientes e um servidor. Aumentar para três ou quatro apenas se houver margem. Carga maior usa processos externos distribuídos.

Unity Personal pode atender ao início se a pessoa/organização cumprir o enquadramento de receita/financiamento abaixo de US$ 200 mil nos últimos 12 meses; conferir os critérios da licença para o caso concreto. [Unity Personal](https://unity.com/products/unity-personal).

## Hospedagem experimental

Estas configurações são pontos de partida para medir um processo de partida, não requisitos garantidos nem especificações mínimas comerciais.

| Uso | CPU sugerida | RAM | Disco | Observação |
|---|---|---|---|---|
| Teste remoto de 4–12 jogadores | 4 vCPU com desempenho consistente | 8 GB | 40–80 GB SSD | VPS pequena para conexão, deploy e profiling |
| Ensaios de 30–60 jogadores | 6–8 vCPU preferencialmente dedicadas | 16 GB | 80–120 GB SSD | Ajustar conforme tick e entidades |
| Candidato a 100 jogadores | 8–16 vCPU dedicadas ou servidor físico moderno | 16–32 GB | 100–200 GB SSD/NVMe | Requer benchmark completo; pode precisar mais ou menos |

Servidor Linux AMD64, inicialmente Ubuntu 24.04 LTS conforme suporte da Unity; sem GPU para a build dedicada. Frequência e desempenho por núcleo importam; muitos vCPUs compartilhados não garantem tick estável. CPU steal, limites de tráfego e região precisam constar da avaliação.

Preferir rede de 1 Gbit/s, baixa latência para o público, acesso UDP público, proteção DDoS adequada a jogos e tráfego suficiente. A região será escolhida pela localização dos jogadores, ainda não informada. CGNAT na residência pode impedir entrada direta; não é necessário expor a máquina de desenvolvimento para hospedar testes públicos.

## Banda: como estimar sem inventar capacidade

Exemplo puramente aritmético: se cada cliente receber em média 20–50 kB/s do servidor, 100 clientes exigirão 2–5 MB/s, ou 16–40 Mbit/s de saída. Em 30 dias de ocupação contínua, isso equivale aproximadamente a 5,2–13 TB decimais de tráfego de saída. Somar entrada, cabeçalhos, retransmissões, picos e eventual voz. A ocupação real pode reduzir o total; ingresso tardio e destruição podem aumentar picos.

Essas taxas por cliente ainda não foram medidas. Medir bytes na interface e no protocolo antes de contratar plano definitivo. Uma porta de 1 Gbit/s com pouca franquia não resolve custo mensal.

## Operação mínima

- Build de servidor dedicada, usuário sem privilégios, portas UDP configuráveis e acesso administrativo restrito.
- Executar como serviço supervisionado com reinício, logs com rotação e limites de armazenamento. Health check deve detectar simulação travada, não só processo aberto.
- Publicar builds com versão de protocolo e rejeitar clientes incompatíveis com mensagem clara. Atualizar entre partidas, mantendo rollback para a versão anterior.
- Métricas de tick, CPU por núcleo, RAM, banda, jogadores, zombies, veículos e objetos destruídos. Alertar a partir de limites observados nos testes.
- Banco/serviço de perfil em rede privada quando adicionados; backups testados. Estado de partida inicialmente pode ser descartado após crash, com política clara ao jogador; não prometer recuperação ainda não implementada.
- Servidores de carga separados do servidor avaliado. Testar conexão externa real, não somente localhost.

Docker é opcional; um executável Linux supervisionado é suficiente para começar. Não precisamos de Kubernetes, cluster ou múltiplas regiões no primeiro teste. Uma VPS de banco/site não deve ser considerada automaticamente adequada ao servidor de jogo.

## Custos e contratação

Não foi selecionado provedor nem levantada cotação comercial. Antes de contratar, comparar preço da configuração medida, tráfego incluído, excesso de banda, CPU dedicada, localização, proteção e backup. Usar: custo mensal = nós de partida + perfil/banco + armazenamento/backup + tráfego excedente + monitoramento/voz.

Somar uma margem de capacidade depois de medir; não preencher o servidor até o limite do tick. Nenhuma configuração da tabela comprova suporte a 100 jogadores. Esse número só deve constar da oferta pública quando os testes da fase 6 passarem.
