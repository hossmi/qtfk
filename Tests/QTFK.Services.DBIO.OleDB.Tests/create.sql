CREATE TABLE persona(
	[id] int IDENTITY(1,1) NOT NULL,
	[nombre] varchar(255) NULL,
	[apellidos] varchar(255) NULL,
	[fecha_nacimiento] datetime NULL,
	[hora_nacimiento] datetime NULL,
 CONSTRAINT [PK_persona] PRIMARY KEY ([id])
) 

GO

CREATE TABLE etiqueta(
	[id] int IDENTITY(1,1) NOT NULL,
	[nombre] varchar(255) NULL,
 CONSTRAINT [PK_etiqueta] PRIMARY KEY ([id] )
)

GO

CREATE TABLE [etiquetas_personas](
	[persona_id] int NOT NULL,
	[etiqueta_id] int NOT NULL,
 CONSTRAINT [PK_etiquetas_personas] PRIMARY KEY (persona_id,etiqueta_id)
) 

GO

ALTER TABLE [etiquetas_personas]  
ADD  CONSTRAINT [FK_etiquetas_personas__etiqueta] 
FOREIGN KEY([etiqueta_id])
REFERENCES [etiqueta] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [etiquetas_personas]  
ADD  CONSTRAINT [FK_etiquetas_personas__persona] 
FOREIGN KEY([persona_id])
REFERENCES [persona] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

