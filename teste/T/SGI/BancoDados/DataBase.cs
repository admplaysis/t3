using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SGI.BancoDados
{
    public class DataBase
    {
        public List<ObjetoDataBase> Objetos { get; set; }
        public DataBase(Tipo.Banco tipoBanco)
        {
            Objetos = new List<ObjetoDataBase>();
            if (tipoBanco == Tipo.Banco.SqlServer)
            {
                Objetos.Add(new ObjetoDataBase() {
                    Tipo = Tipo.Objeto.View,
                    Nome = "V_CLP_MEDICOES",
                    Comando = @"CREATE VIEW [dbo].[V_CLP_MEDICOES]
                                AS SELECT MAQUINA_ID, MIN(DATA) AS DATA_INI, MAX(DATA) AS DATA_FIM, SUM(QTD) AS QTD, GRUPO, STATUS, URN_ID, URM_ID 
                                ,MED_ID, MED_OBSERVACOES, OCO_ID, ORD_ID
                                FROM T_CLP_MEDICOES CLM
                                LEFT JOIN T_MEDICOES TM ON TM.MAQ_ID = CLM.MAQUINA_ID AND TM.MED_GRUPO = CLM.GRUPO
                                GROUP BY MAQUINA_ID, GRUPO, STATUS, URN_ID, URM_ID ,MED_ID, MED_OBSERVACOES, OCO_ID, ORD_ID"
                });
                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.View,
                    Nome = "vw_SGI_PARAMETRO_RELMEDICOES",
                    Comando = @"CREATE VIEW [dbo].[vw_SGI_PARAMETRO_RELMEDICOES]
                                AS
                                SELECT    max(med.T_DATA)T_DATA,med.MET_ID,ind.NEG_ID,ind.IND_ID,med.UNI_ID UNID,un.UN UN, ind.IND_DESCRICAO, LEFT(med.T_DATAMEDICAO, 6) AS Mes, SUM(CONVERT(float, ISNULL(med.T_VALORMEDIDO,'0'))) AS Valor, CONVERT(float, ISNULL(met.MET_ALVO,'0')) AS META, 
                                isnull(max(CONVERT(float, ISNULL(med.AC,0))),'0') AS ValorAc,
                                                      CONVERT(float, ISNULL(met.MET_ALVO,'0')) AS METAMES, CONVERT(float, ISNULL(met.MET_ALVO,'0')) * CONVERT(float, ISNULL(SUBSTRING(med.T_DATAMEDICAO, 5, 2),'0')) AS METACU
                                                      ,ISNULL(case when SUM(MED_PONDERACAO) > 0 then sum(MED_PONDERACAO*CONVERT(float,ISNULL(T_VALORMEDIDO,'0')))/SUM(MED_PONDERACAO) else 0 end,'0') MED_PONDERACAO
                                FROM         dbo.Tr_Medicoes AS med 
                                left join T_Unidade un on un.UNI_ID = med.UNI_ID
                                INNER JOIN dbo.T_Metas AS met ON met.MET_ID = med.MET_ID 
                                INNER JOIN dbo.T_Indicadores AS ind ON ind.IND_ID = met.IND_ID
                                GROUP BY ind.IND_DESCRICAO,med.UNI_ID, LEFT(med.T_DATAMEDICAO, 6), met.MET_ALVO, SUBSTRING(med.T_DATAMEDICAO, 5, 2), ind.IND_ID,ind.NEG_ID,med.MET_ID,un.UN"
                });


                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.Trigger,
                    Nome = "TGR_CLP_MEDICOES_01",
                    Comando = @"USE [SGI]
ALTER TRIGGER [dbo].[TGR_CLP_MEDICOES_01] 
ON [dbo].[T_CLP_MEDICOES] 
FOR INSERT, UPDATE 
AS 
  BEGIN 
      DECLARE @ID         INT, 
              @U_ID       INT, 
              @MAQUINA_ID VARCHAR(10), 
              @DATA_INI       DATETIME, 
              @DATA_FIM       DATETIME, 
              @DATA_ULTIMA_PECA_PRODUZIDA       DATETIME, 
			  @TEMPO_MINIMO_PARADA INT,
			  @UP_QTD INT,
			  @UP_GRUPO INT,
			  @QTD        INT, 
              @ID_LOTE_CLP INT,
			  @U_QTD      INT, 
              @GRUPO      INT, 
              @U_GRUPO    INT, 
              @STATUS     INT, 
              @URN_ID     VARCHAR(1), 
              @U_URN_ID   VARCHAR(1), 
              @URM_ID     VARCHAR(1), 
              @U_URM_ID   VARCHAR(1),
			  @PRO_ID	 VARCHAR(30),
			  @U_PRO_ID	 VARCHAR(30),

			  @ORD_ID     VARCHAR(30),
			  @U_ORD_ID     VARCHAR(30),
			  
			  @ROT_SEQ_TRANFORMACAO int,
			  @FPR_SEQ_REPETICAO int,
			  @U_ROT_SEQ_TRANFORMACAO int,
			  @U_FPR_SEQ_REPETICAO int,

			  @FASE    VARCHAR(1),
			  @U_FASE    VARCHAR(1),
			  @INCREMENTOU    VARCHAR(1),
			  @U_PERFORMANCE FLOAT,
			  @TAR_PROXIMA_META_PERFORMANCE FLOAT,
			  @OCO_ID VARCHAR(10),
			  @DESCONSIDERA_PARADA VARCHAR(1),
			  @U_OCO_ID VARCHAR(10),
			  @PERFORMANCE FLOAT 

      SELECT @ID = ID, 
             @MAQUINA_ID = maquina_id, 
             @DATA_INI = DATA_INI, 
             @DATA_FIM = DATA_FIM, 
             @QTD = QTD, 
             @GRUPO = grupo, 
             @STATUS = status, 
             @URN_ID = urn_id, 
             @URM_ID = urm_id,
			 @ORD_ID = ORD_ID,
			 @ROT_SEQ_TRANFORMACAO = ROT_SEQ_TRANFORMACAO,
			 @FPR_SEQ_REPETICAO = FPR_SEQ_REPETICAO,
			 @ID_LOTE_CLP = ID_LOTE_CLP,
			 @PERFORMANCE = ISNULL(QTD / (CASE WHEN  DATA_INI = DATA_FIM THEN 1 ELSE DATEDIFF(SECOND, DATA_INI,DATA_FIM) END),-1) 
      FROM   inserted 
IF @ID_LOTE_CLP >= 0 -- MENOR QUE ZERO NAO ENTRA AQUI POIS SE TRATA DE INSERT MANUAL NO OPROCESSO DE SPLIT
BEGIN
      -- Agrupar na sequencia de data e hora quando a informação for zero   ou diferente de zero    
      SELECT TOP 1 @U_URN_ID = urn_id, 
                   @U_URM_ID = urm_id, 
                   @U_ID = id, 
                   @U_FASE = FASE,
				   @U_QTD = qtd, 
                   @U_GRUPO = grupo,
				   @U_OCO_ID= OCO_ID,
				   @U_ORD_ID = ORD_ID,
				   @ROT_SEQ_TRANFORMACAO = ROT_SEQ_TRANFORMACAO,
				   @FPR_SEQ_REPETICAO = FPR_SEQ_REPETICAO,
			 
				   @U_PERFORMANCE = ISNULL(QTD / (CASE WHEN  DATA_INI = DATA_FIM THEN 1 ELSE DATEDIFF(SECOND, DATA_INI,DATA_FIM) END),-1) 
      FROM   t_clp_medicoes (nolock) 
      WHERE  id < @ID 
             AND MAQUINA_ID = @MAQUINA_ID 
      ORDER  BY id DESC
	  PRINT 'ULTIMO TURNO'
	  PRINT @U_URN_ID
	  --
	  SELECT @URN_ID = urn_id, 
             @URM_ID = urm_id 
      FROM   t_itens_calendario I(nolock) 
             INNER JOIN t_maquina M 
                     ON M.cal_id = I.cal_id 
      WHERE  @DATA_INI BETWEEN ica_data_de AND ica_data_ate 
             AND M.maq_id = @MAQUINA_ID 

			 PRINT 'TURNO CALENDARIO '
			PRINT @URN_ID


	SET @GRUPO = 1 -- POR PADRAO 
	SET @INCREMENTOU = 'N'
		SET @DESCONSIDERA_PARADA = 'N'
		IF @ORD_ID = '' OR @ORD_ID IS NULL  -- CASO CLP PERCA A OP O SISTEMA UTILIZA A ULTIMA 
		BEGIN
			IF @U_ORD_ID IS NULL 
			BEGIN
				SET @U_ORD_ID =''
				SET @U_ROT_SEQ_TRANFORMACAO = ''
				SET @U_FPR_SEQ_REPETICAO = ''
				SET @U_PRO_ID	 = ''
			END
			SET @ORD_ID = @U_ORD_ID
			SET @ROT_SEQ_TRANFORMACAO = @U_ROT_SEQ_TRANFORMACAO
			SET @FPR_SEQ_REPETICAO	  = @U_FPR_SEQ_REPETICAO
			SET @PRO_ID	 =@U_PRO_ID	

		END 
		
		IF @OCO_ID = ''
			SET @OCO_ID = @U_OCO_ID


	/*FASES
	1 - SETUP			= QUANTIDADES ZERO 
	2 - SETUP AJUSTE	= QUANTIDADES DIFERENTE DE ZERO STARTA SETUP AJUSTE E QUANTIDADES A CIMA DOS 80% ENCERRA 
	3 - PRODUZINDO	= QUANTIDADE A CIMA DE 80% DA META STARTA
	4 - PARADAS		= QUANTIDADE ZERADA*/

	-- FASE ANTERIOR 
	SET @FASE = (SELECT COUNT(*) FROM (select DISTINCT GRUPO from T_CLP_MEDICOES M 
	WHERE ORD_ID = @ORD_ID AND PRO_ID = @PRO_ID	AND ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO  AND FPR_SEQ_REPETICAO = @FPR_SEQ_REPETICAO
	AND MAQUINA_ID = @MAQUINA_ID  AND  id < @ID  ) T)
	
	
	SET @TEMPO_MINIMO_PARADA = 60
	select TOP 1 @TEMPO_MINIMO_PARADA = ISNULL(TAR_PARAMETRO_TIME_WORK_STOP_MACHINE,60) , @TAR_PROXIMA_META_PERFORMANCE = TAR_PROXIMA_META_PERFORMANCE from T_TARGET_PRODUTO T
	WHERE PRO_ID = @PRO_ID AND	T.MAQ_ID = @MAQUINA_ID AND ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO
	ORDER BY TAR_ID DESC

	PRINT 'TEMPO MINIMO'
	PRINT @TEMPO_MINIMO_PARADA

	IF @U_FASE > @FASE
		SET @FASE = @U_FASE
	
	IF @FASE = '0' 
		SET @FASE = '1'
	-- FASE ATUAL 
	IF @FASE = '1' -- VERIFICA SE ESTA NA FAZE DOIS 
	BEGIN 
		IF @QTD >0 
		BEGIN
			SET @FASE = '2'
			SET @GRUPO = @U_GRUPO+1
			SET @INCREMENTOU = 'S'
		END
	END
	IF @FASE = '2' -- VERIFICA SE ESTA NA FASE 3 
	BEGIN 
		-- como teste para que se quebre novamente o grupo desta forma sempre que ouverem variacoes de velocidade o sistema ira quebrar o grupo
		IF (@PERFORMANCE  > (@TAR_PROXIMA_META_PERFORMANCE * 0.8))-- and (@U_PERFORMANCE  < (@TAR_PROXIMA_META_PERFORMANCE * 0.8)) 
		Begin
			SET @GRUPO = @U_GRUPO+1 
			SET @FASE = '3'
			SET @INCREMENTOU = 'S'
			PRINT @GRUPO
		END
		ELSE
		BEGIN
			IF @INCREMENTOU = 'N'
				SET @GRUPO = @U_GRUPO 
		END

	End
	IF @FASE > '2'
	BEGIN 
		PRINT 'MAIOR '
		IF @INCREMENTOU = 'N'
			SET @GRUPO = @U_GRUPO 
	END 
	PRINT @GRUPO
	PRINT @FASE
	PRINT 'FFFF'
		IF @U_QTD = 0 AND @QTD = 0 
		begin 
			PRINT '222 FFFF'
	
			-- testa se a quantidade anterior zero fas parte de uma pequena parada
			-- SE FIZER NAO MUDA GRUPO

			SET @DATA_ULTIMA_PECA_PRODUZIDA = '1970-01-01 00:00:00.000'
			SELECT TOP 1 @DATA_ULTIMA_PECA_PRODUZIDA = DATA_FIM, @UP_QTD = QTD,@UP_GRUPO = GRUPO FROM [dbo].[T_CLP_MEDICOES]CM 
			WHERE ID < @ID  AND MAQUINA_ID = @MAQUINA_ID AND QTD <> 0 
			--AND ORD_ID = @ORD_ID AND PRO_ID = @PRO_ID AND ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO AND FPR_SEQ_REPETICAO = @FPR_SEQ_REPETICAO
			ORDER  BY ID DESC 
			PRINT @MAQUINA_ID
			PRINT @ORD_ID
			PRINT @PRO_ID
			PRINT @ROT_SEQ_TRANFORMACAO
			PRINT @FPR_SEQ_REPETICAO

			PRINT @DATA_ULTIMA_PECA_PRODUZIDA
			PRINT @DATA_FIM
			PRINT DATEDIFF(SECOND, @DATA_ULTIMA_PECA_PRODUZIDA,@DATA_FIM)
			PRINT @TEMPO_MINIMO_PARADA
			PRINT @UP_GRUPO 
			PRINT @U_GRUPO

			IF (@UP_GRUPO = @U_GRUPO) AND (DATEDIFF(SECOND, @DATA_ULTIMA_PECA_PRODUZIDA,@DATA_FIM) > @TEMPO_MINIMO_PARADA)
			BEGIN
				SET @GRUPO = @U_GRUPO+1 
				PRINT 'SKDJFKJSDFKJSH'
			END
			ELSE
			BEGIN
				PRINT 'SSSSSSSSSSSSSSSSSSSS'
				IF @INCREMENTOU = 'N'
					SET @GRUPO = @U_GRUPO 
				SET @DESCONSIDERA_PARADA = 'S'
			END
		end 
		IF @U_QTD <> 0 AND @QTD <> 0 
		BEGIN 
			-- AQUI TESTA SE PERMANECCE EM SETUP AJUSTE   COUNT = 2 SE SIM TESTA ALMENTE O DE PERFORMANCE SE TEVE ALMENTE 
			-- QUEBRA GRUPO JA TA TRATADO A BAIXO SO MUDAR = 0 PARA = 1 SETUP AJUSTE 
			IF @INCREMENTOU = 'N'
				SET @GRUPO = @U_GRUPO 
			-- CASO JA TENHA SAIDO DO SETUP IGNORA ESSE COMANDO SO VALE PARA QUANTIDADES QUE AINDA ESTAO EM SETUP
			--IF ((select COUNT(*) from T_CLP_MEDICOES M WHERE ORD_ID = @ORD_ID AND MAQUINA_ID = @MAQUINA_ID AND GRUPO = (@U_GRUPO-1) )= 1 ) --0.0 )
			/*
			IF @FASE=2 
			BEGIN -- SIGNIFICA QUE ESTA EM SETUP E DEVE VERIFICAR SE A PERFORMANCE ULTRAPASOU 80% 
				select @TAR_PROXIMA_META_PERFORMANCE = TAR_PERFORMANCE from T_TARGET_PRODUTO T
				INNER JOIN T_ORDEM_PRODUCAO O ON O.PRO_ID = T.PRO_ID  
				WHERE O.ORD_ID = @ORD_ID AND T.MAQ_ID = @MAQUINA_ID
				-- como teste para que se quebre novamente o grupo desta forma sempre que ouverem variacoes de velocidade o sistema ira quebrar o grupo
				IF (@PERFORMANCE  > (@TAR_PROXIMA_META_PERFORMANCE * 0.8))-- and (@U_PERFORMANCE  < (@TAR_PROXIMA_META_PERFORMANCE * 0.8)) 
				Begin
					SET @GRUPO = @U_GRUPO+1 
				end
			END*/
			
		END 
		IF ( @U_QTD = 0 AND @QTD <> 0 )
		BEGIN
			--TESTA SE A QUANTIDADE AINDA NAO ATINGIL 80% DE PERFORMANCE NESTE CASO PERMANECE EM SETUP
			-- teste performance caso a velocidade seja de setup ajuste criara uma linha de setup ajuste caso 
			/*select @TAR_PROXIMA_META_PERFORMANCE = TAR_PERFORMANCE from T_TARGET_PRODUTO T
			INNER JOIN T_ORDEM_PRODUCAO O ON O.PRO_ID = T.PRO_ID  
			WHERE O.ORD_ID = @ORD_ID AND T.MAQ_ID = @MAQUINA_ID
			-- como teste para que se quebre novamente o grupo desta forma sempre que ouverem variacoes de velocidade o sistema ira quebrar o grupo
			-- CASO JA TENHA SAIDO DO SETUP IGNORA ESSE COMANDO SO VALE PARA QUANTIDADES QUE AINDA ESTAO EM SETUP
			IF ((@PERFORMANCE  < (@TAR_PROXIMA_META_PERFORMANCE * 0.8)) AND 
			((select COUNT(*) from T_CLP_MEDICOES M WHERE ORD_ID = @ORD_ID AND MAQUINA_ID = @MAQUINA_ID AND GRUPO = (@U_GRUPO-1) )= 0.0))
			Begin
				SET @GRUPO = @U_GRUPO 
			end
			else
			begin
				*/
				-- testa se a quantidade anterior zero fas parte de uma pequena parada
				-- SOMENTE MUDA O GRUPO SE O REGISTRO NAO FIZER PARTE DE UMA PEQUENA PARADA
				SET @DATA_ULTIMA_PECA_PRODUZIDA = '1970-01-01 00:00:00.000'
				SET @UP_GRUPO=0
				SELECT TOP 1 @DATA_ULTIMA_PECA_PRODUZIDA = DATA_FIM, @UP_QTD = QTD,@UP_GRUPO = GRUPO FROM [dbo].[T_CLP_MEDICOES]CM 
				WHERE ID < @ID  AND MAQUINA_ID = @MAQUINA_ID AND ORD_ID = @ORD_ID AND PRO_ID = @PRO_ID	AND QTD <> 0 AND 
				ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO AND FPR_SEQ_REPETICAO = @FPR_SEQ_REPETICAO
				ORDER  BY ID DESC 
				IF (DATEDIFF(SECOND, @DATA_ULTIMA_PECA_PRODUZIDA,@DATA_FIM) > @TEMPO_MINIMO_PARADA) 
				BEGIN
					IF @INCREMENTOU = 'N'
						SET @GRUPO = @U_GRUPO+1
					SET @INCREMENTOU = 'S'
 				END
				ELSE
				BEGIN
					IF @INCREMENTOU = 'N'
						SET @GRUPO = @U_GRUPO 
					SET @DESCONSIDERA_PARADA = 'S'
				END 
			--End
		END	
		
		/*IF ( @U_QTD <> 0 AND @QTD = 0 )  
		BEGIN
			/*--delete  from T_CLP_MEDICOES   delete from T_FEEDBACK
				INSERT INTO [T_CLP_MEDICOES](MAQUINA_ID, DATA_INI, DATA_FIM, QTD, ORD_ID, ID_LOTE_CLP)
			  VALUES('MQ003',	'2018-04-04 00:00:00',	'2018-04-04 00:07:59',	6,	'OP001', 1 )
			
			INSERT INTO [T_CLP_MEDICOES](MAQUINA_ID, DATA_INI, DATA_FIM, QTD, ORD_ID, ID_LOTE_CLP)
			VALUES('MQ001',	'2018-06-06 08:00:01.000',	'2018-06-06 08:00:06.000',	0,	'', 176 )
			
			
			SELECT * FROM [SGI].[dbo].[T_CLP_MEDICOES]
			  where MAQUINA_ID ='MQ003' 
			  ORDER BY ID DESC*/
	     	-- TRATA TEMPO MINIMO PARA REGISTRAR UMA PARADA 
			SET @DATA_ULTIMA_PECA_PRODUZIDA = '1970-01-01 00:00:00.000'
			SELECT TOP 1 @DATA_ULTIMA_PECA_PRODUZIDA = DATA_FIM  FROM [dbo].[T_CLP_MEDICOES]CM 
			WHERE ID < @ID  AND MAQUINA_ID = @MAQUINA_ID AND ORD_ID = @ORD_ID AND QTD <> 0
			ORDER  BY ID DESC 
			IF DATEDIFF(SECOND, @DATA_ULTIMA_PECA_PRODUZIDA,@DATA_FIM) > @TEMPO_MINIMO_PARADA  -- ATUALMENTE TEMPO MINIMO [E DE UM MINUTO 
			BEGIN 
				SET @GRUPO = @U_GRUPO + 1
			END
			ELSE
			BEGIN
				SET @DESCONSIDERA_PARADA = 'S'
				SET @GRUPO = @U_GRUPO
			END
			
		END*/
		
		
		
		-- VAI PULAR UM ID CASO O TURNO TENHO MUDADO E CONENCIDENTEMENTE TENHO MUDADO QUANTIDADE       ALTERADO DEPOIS DAS FASES  
		-- SOMENTE QUEBRA TURNO SE TURNO ESTIVER PREENCHIDO NO CALENDARIO   
		--EM CASO DE UM TUNO SO O SISTEMA NAO QUEBRA QUANDO VEM DE UM CALENDARIO SEM TURNO DEFINIDO
		
		-- NAO QUEBRA QUANDO VEM DE TURNO SEM EXPEDIENTE     IF ((@U_URN_ID IS NOT NULL AND @U_URN_ID IS NOT NULL) OR (@U_URN_ID <> '' AND @U_URN_ID <> '')) 
		IF (ISNULL(@U_URN_ID,'') <> ISNULL(@URN_ID,'') OR ISNULL(@U_URM_ID,'') <> ISNULL(@URM_ID,'')   )  AND @INCREMENTOU = 'N'
		BEGIN
			IF (@U_URN_ID IS NULL AND @U_QTD <> 0)  -- NAO QUEBRAR 
				PRINT ''
			ELSE
			IF @INCREMENTOU = 'N'
				SET @GRUPO = @U_GRUPO+1
			SET @INCREMENTOU = 'S'
		END
	
		 
		IF ISNULL(@U_GRUPO,'') <> ISNULL(@GRUPO,'') -- MUDOU GRUPO ZERA OCORRENCIA 
		BEGIN
			SET @OCO_ID = ''
		END
		PRINT @GRUPO 
		-- ENTRAN NESTAS LINHAS SETUPS OU PARADAS FALTA (TRATAR PEQUENAS PARADAS TAMBEM APOS FIM DE SETUP OU FIM DE PARADA)
		
		/*
		IF @ORD_ID <> '' AND  @U_ORD_ID <> '' -- SEM OP NAO FARA CLASSIFICACOES DE NEMNHUM GENERO EM RELACAO A PERFORMACE APOS PREENCHIMENTO O LOTE SERA AVALIADO COMFORME REGISTRO CORRENTE 
		BEGIN 
			IF (@QTD = 0) 
			BEGIN
				-- 
				IF @U_OCO_ID = '' AND @ORD_ID = ''
				   SET @OCO_ID = '1.1'
			END 
			ELSE
			BEGIN
				-- teste performance caso a velocidade seja de setup ajuste criara uma linha de setup ajuste caso 
				select @TAR_PROXIMA_META_PERFORMANCE = TAR_PERFORMANCE from T_TARGET_PRODUTO T
				INNER JOIN T_ORDEM_PRODUCAO O ON O.PRO_ID = T.PRO_ID  
				WHERE O.ORD_ID = @ORD_ID AND T.MAQ_ID = @MAQUINA_ID
				-- como teste para que se quebre novamente o grupo desta forma sempre que ouverem variacoes de velocidade o sistema ira quebrar o grupo
				IF (@PERFORMANCE  < (@TAR_PROXIMA_META_PERFORMANCE * 0.8))-- and (@U_PERFORMANCE  < (@TAR_PROXIMA_META_PERFORMANCE * 0.8)) 
				begin
					set @OCO_ID = '1.2'
					-- caso NAO EXISTA REGISTRO DE PRODUCAO NORMAL PARA ESTA OP 
					if ((select count(*) from T_CLP_MEDICOES WHERE ORD_ID <> '' AND ORD_ID = @ORD_ID AND MAQUINA_ID = @MAQUINA_ID and LEFT(OCO_ID,1) = '4') = 0.0)
					BEGIN
						set @OCO_ID = '1.2'
					END
					ELSE
					BEGIN 
						set @OCO_ID = '4.1'
					END 
				end
				PRINT '000000000000000000'
				-- se a performance atual for maior que 80% do targat e a performance anterior for menor quebra o grupo 
				-- isso para que se quebra uma unica ves o grupo por setup ajuste 
				IF (@PERFORMANCE  > (@TAR_PROXIMA_META_PERFORMANCE * 0.8)) --AND (@DESCONSIDERA_PARADA <> 'S') 
				--AND (@U_PERFORMANCE  < (@TAR_PROXIMA_META_PERFORMANCE * 0.8)) 
				begin 
				PRINT '000000000000000000'
					set @OCO_ID = '4.1' -- NESTE MOMENTO AINDA NAO ESTA NA META VAMOS ANALISAR SE IREMOS QUEBRAR OU NAO
					if ((select count(*) from T_CLP_MEDICOES 
					WHERE ORD_ID = @ORD_ID AND MAQUINA_ID = @MAQUINA_ID 
					and LEFT(OCO_ID,1) = '4') = 0.0)
					BEGIN
						SET @GRUPO = @U_GRUPO + 1
						PRINT 'VEIO '
					END
				end
		   END
	   	END */
		-- ATUALIZA REGSTRO ATUAL 
		SET @OCO_ID = ''
		UPDATE t_clp_medicoes 
		SET    grupo = Isnull(@GRUPO, 1), 
		urn_id = @URN_ID, 
		urm_id = @URM_ID, 
		ORD_ID = @ORD_ID,
		PRO_ID = @PRO_ID,	 
		ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO,
		FPR_SEQ_REPETICAO = @FPR_SEQ_REPETICAO,
		OCO_ID = @OCO_ID,
		FASE = @FASE,
		CLP_EMISSAO = GETDATE() 
		WHERE  id = @ID 

		-- ATUALIZA OCORRENCIA E OP PARA O GRUPO
		IF @QTD <> 0 -- ACOMTESE QUE QUANDO ESTAMOS COM QUANTIDADE ZERO EM GRUPOS QUE TOLERAM PARADAS DE MENOS DE X TEMPO   O SISTEMA PERDE AS OCORRENCIAS  
			UPDATE T_CLP_MEDICOES SET  URN_ID= @URN_ID , URM_ID =@URM_ID, PRO_ID = @PRO_ID, ORD_ID = @ORD_ID, ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO , FPR_SEQ_REPETICAO = @FPR_SEQ_REPETICAO WHERE  GRUPO = @GRUPO AND MAQUINA_ID = @MAQUINA_ID
		ELSE
			UPDATE T_CLP_MEDICOES SET  URN_ID= @URN_ID , URM_ID =@URM_ID, PRO_ID = @PRO_ID, ORD_ID = @ORD_ID , ROT_SEQ_TRANFORMACAO = @ROT_SEQ_TRANFORMACAO , FPR_SEQ_REPETICAO = @FPR_SEQ_REPETICAO WHERE  GRUPO = @GRUPO AND MAQUINA_ID = @MAQUINA_ID
		
  /*      - OK Agrupar na sequencia de data e hora quando a informação for zero   ou diferente de zero      - OK Quebrar grupo quando o mesmo estiver entre turnos       - OK Gravar turno e turma baseado no calendario             - Tratar linhas com variação entre zero e quantidade com intervalos muito curtos               - opção 1  parametrizar tempo minimo   exemplo so considera maquina parada se depois de 1 minuto sem quantidade so considera produzindo depois de 1 minuto com quantidade       - dia da turma    quando comesa e termina o dia da turma  */
END
END




"
                });









                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.View,
                    Nome = "",
                    Comando = @""
                });
                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.View,
                    Nome = "",
                    Comando = @""
                });
                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.View,
                    Nome = "",
                    Comando = @""
                });
                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.View,
                    Nome = "",
                    Comando = @""
                });
                Objetos.Add(new ObjetoDataBase()
                {
                    Tipo = Tipo.Objeto.View,
                    Nome = "",
                    Comando = @""
                });
            }
        }
    }
}