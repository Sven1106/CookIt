-- create temporary table for deleted IDs
CREATE TABLE WordDeleteId (
    Id Varchar(255) PRIMARY KEY
)
CREATE TABLE SynsetDeleteId (
    Id Varchar(255) PRIMARY KEY
)

-- Words that aren't Noun
INSERT INTO WordDeleteId(Id)
SELECT Word.Id
FROM Word
WHERE Pos != 'Noun'

-- Synset that aren't Animal or Comestible
INSERT INTO SynsetDeleteId(Id)
SELECT Synset.Id
FROM Synset
WHERE Synset.Ontological_type
NOT LIKE '%Animal%' AND Synset.Ontological_type NOT LIKE '%Comestible%'

-- DELETE WORDSENSE
DELETE Wordsense
FROM Wordsense
WHERE Wordsense.SynsetId IN (
    SELECT SynsetDeleteId.Id
    FROM SynsetDeleteId
)OR Wordsense.SynsetId IN ( -- Delete where Synset doesn't exist
	SELECT DISTINCT Wordsense.SynsetId
	FROM Wordsense 
	WHERE NOT EXISTS (
		SELECT *
		FROM Synset 
		WHERE Synset.Id = Wordsense.SynsetId
	)
)

-- DELETE SYNSET
DELETE Synset
FROM Synset
WHERE Synset.Id IN (
    SELECT SynsetDeleteId.Id
    FROM SynsetDeleteId
)

-- ADD Deleted Wordsense.WordId to WordDeleteId
INSERT INTO WordDeleteId(Id)
SELECT DISTINCT Word.id
FROM Word 
WHERE NOT EXISTS (
    SELECT *
    FROM Wordsense 
    WHERE Wordsense.WordId = Word.id
)
AND NOT EXISTS(
		SELECT WordDeleteId.Id
		FROM WordDeleteId
		WHERE WordDeleteId.Id = Word.id
)

-- DELETE WORD
DELETE Word
FROM Word
WHERE Word.Id IN (
    SELECT WordDeleteId.Id
    FROM WordDeleteId
)

DROP TABLE WordDeleteId
DROP TABLE SynsetDeleteId