
CREATE FUNCTION [FT].[ConsonantTerm] (@Term varchar(50)) 
RETURNS varchar(10) 
AS 
BEGIN 
	DECLARE @MaxLen tinyint = 5,@ConsonantTerm varchar(50) = '' 
	
	DECLARE @CurrCh char(1), @NextCh char(1), @PrevCh char(1), @ConsCh char(1) = '', @Next2Ch char(2), @Prev2Ch char(2)
	
	DECLARE @i tinyint = 1, @VowelFlag bit = 1
	
	DECLARE @Len int 

	SET @Len = Len(@Term)

	IF ISNUMERIC(@Term) = 0 AND LEN(@Term) > 2 
	BEGIN 
		SET @Term =           REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@Term, '-',''),'[',''),'\',''),':',''),'$',''), '#',''), '"',''),'@',''),'/',''),'*',''),'!',''),'_',''),' ','') 
		IF LEN(@Term) > 2 
		BEGIN 
		IF SUBSTRING( @Term, @Len-2, 3) = 'IES' 
			SET @Term = SUBSTRING( @Term, 1, @Len-3) + 'YS' 
		ELSE IF SUBSTRING( @Term, @Len-2, 3) = 'GHT' 
			SET @Term = SUBSTRING( @Term, 1, @Len-3) + 'T' 
		ELSE IF SUBSTRING( @Term, @Len-1, 2) = 'SS' OR SUBSTRING( @Term, @Len-1, 2) = 'ES' 
			SET @Term = SUBSTRING( @Term, 1, @Len-1) + 'S' 
		ELSE IF SUBSTRING( @Term, @Len, 1) = 'S'
			SET @Term = SUBSTRING( @Term, 1, @Len-1)  --strip trailing S
			
		SET @CurrCh = SUBSTRING(@Term, @i, 1)

		WHILE @i <= LEN(@Term) AND Len(@ConsonantTerm) < @MaxLen 
		BEGIN
			
			IF @i < LEN(@Term)	
			BEGIN 
				SET @NextCh = SUBSTRING(@Term, @i+1, 1)
				SET @Next2Ch = SUBSTRING(@Term, @i+1, 2)
			END
			ELSE 
			BEGIN 
				SET @NextCh = ''
				SET @Next2Ch=''
			END

	--	SELECT Term = @Term, ConsonantTerm = @ConsonantTerm, i = @i, currch = @CurrCh, nextch = @NextCh, consch = @ConsCh, @PrevCh
			IF	
				@CurrCh = '1' OR @CurrCh = '2' OR @CurrCh = '3' OR @CurrCh = '4' OR @CurrCh = '5' OR @CurrCh = '6' OR @CurrCh = '7' OR @CurrCh = '8' OR @CurrCh = '9' OR @CurrCh = '0' 
				OR @CurrCh = '-' OR @CurrCh = '[' OR @CurrCh = ']' OR @CurrCh = '\' OR @CurrCh = '/' OR @CurrCh = ':' OR @CurrCh = '$' OR @CurrCh = '#' OR @CurrCh = '"' OR @CurrCh = '''' OR @CurrCh = '*' OR @CurrCh = '_' OR @CurrCh = ' '
				SET @CurrCh = @NextCh
			ELSE IF (@i > 1 AND  @CurrCh = 'Y' AND @VowelFlag = 1)
			BEGIN 
				SET @ConsCh = 'A'
				SET @ConsonantTerm = @ConsonantTerm + @ConsCh
				SET @VowelFlag = 0
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
			END
			ELSE IF (@CurrCh = 'A' OR @CurrCh = 'E' OR @CurrCh = 'I' OR @CurrCh = 'O' OR @CurrCh = 'U' ) 
				BEGIN 

					IF @VowelFlag = 1
					BEGIN 
						IF (@ConsCh = 'm' AND @CurrCh = 'a' AND @NextCh = 'c') OR (@ConsCh = 'm' AND @CurrCh = 'a' AND @NextCh = 'q')
							SET @ConsCh = 'C'
	--					IF @ConsCh = 'o' AND @NextCh = 'o' SET @ConsCh = 'U'
						ELSE
							SET @ConsCh = 'A'
						SET @ConsonantTerm = @ConsonantTerm + @ConsCh
						SET @VowelFlag = 0 
					END
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
				END
			ELSE
			BEGIN

				IF (@CurrCh = @NextCh) 
					OR (@CurrCh = 'T' AND @NextCh = 'H' )
					OR (@CurrCh = 'N' AND @NextCh = 'M' )
					OR (@CurrCh = 'M' AND @NextCh = 'P' )
					OR (@CurrCh = 'W' AND @NextCh = 'H' )
				BEGIN
					IF @ConsCh <> @CurrCh
					BEGIN 
						SET @ConsonantTerm = @ConsonantTerm + @CurrCh
						SET @ConsCh = @CurrCh
					END
				END
				ELSE IF 
					(@CurrCh = 'C' AND @NextCh = 'K' )
					OR (@CurrCh = 'S' AND @NextCh = ' ' )
					OR (@CurrCh = 'K' AND @NextCh = 'N' )
					OR (@CurrCh = 'W' AND @NextCh = 'R' )
					OR (@CurrCh = 'C' AND @NextCh = 'Q' )
				BEGIN
					IF @ConsCh <> @NextCh AND @NextCh <> ' ' 
					BEGIN 
						SET @ConsonantTerm = @ConsonantTerm + @NextCh
						SET @ConsCh = @NextCh
					END
					SET @CurrCh = @NextCh
				END 
				ELSE IF (@CurrCh = 'J' AND @NextCh = 'E' )
				BEGIN
					SET @CurrCh = @NextCh
					SET @ConsCh = 'G'
					SET @ConsonantTerm = @ConsonantTerm + @ConsCh
				END
				ELSE IF (@CurrCh = 'C' AND (@NextCh = 'O'  OR @NextCh = 'U' OR @NextCh = 'L'))

				BEGIN
					SET @CurrCh = @NextCh
					SET @ConsCh = 'K'
					SET @ConsonantTerm = @ConsonantTerm + @ConsCh
				END
				ELSE IF @PrevCh = 'a' AND @CurrCh = 'r'  AND @NextCh = 'b'
				BEGIN 
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
				END 
				ELSE IF	(@CurrCh = 'h' AND @NextCh = '' AND  @PrevCh = 'a' )
				BEGIN 
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
				END 
				ELSE IF	(@CurrCh = 'h' AND @NextCh = '' AND  @PrevCh = 'a' )
				BEGIN 
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
				END 
				ELSE IF	(@CurrCh = 'g' AND @Prev2Ch = 'in' )
				BEGIN 
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
				END 
				ELSE
				BEGIN
					IF @ConsCh <> @CurrCh AND @CurrCh <> ' '
					BEGIN 
						SET @ConsonantTerm = @ConsonantTerm + @CurrCh
						SET @ConsCh = @CurrCh
					END
					SET @Prev2Ch = @PrevCh + @CurrCh
					SET @PrevCh = @CurrCh
					SET @CurrCh = @NextCh
				END
			END
			SET @i = @i + 1
			END
		END 
		IF LEN(@ConsonantTerm) < 2 SET @ConsonantTerm = '' 
	END 
	RETURN LEFT(@ConsonantTerm,5) 
END 
