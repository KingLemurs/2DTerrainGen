# 2D TERRAIN GENERATOR

This tool generates 2D terrain in the form of Unity's built in Tilemaps, which are an array of tiles placed within a scene.

In order to run the tool, all you need are the two scripts located in Assets/Scripts: PerlinNoiseMap.cs and WeightTable.cs. Once you have those two scripts in your project, simply make a new rectangular grid in a scene, attach the PerlinNoiseMap script onto an object and pass in the grid. Some example tiles and weight tables are provided but not neccesary. The script will run automatically once play is pressed, but you can easily redirect what is in the Start() function so the map generates whenever you want.

Parameters:

Seed - An integer that changes the RNG seed for the generator
  * Must be in between -1,000,000 and 1,000,000

Map Width & Height - Integers that change the output map size

Tilemap - Changes which tilemap the output generates to

Layer Data - The CORE part of this generator. The generator is made up of "Layers", which determine how tiles are placed on the output map and the ORDER in which they are placed. Tiles from Layer element 0 will be placed BEFORE tiles from Layer element 1. Each Layer consist of different elements:
  1. Layer Type - An enum that determines how the tiles overlay onto the output map.
     * NORMAL: The layer will completely overlay onto every available tile of the map.
     * ADD: The layer will replace tiles on top of existing layers depending on the Layer Threshold.
     * SUBTRACT: The layer will remove tiles from existing layers with empty space. IF you add a weight table to this layer you will also add tiles to the map.
       
  2. Layer Threshold - A float between 0 - 1 that represents a % chance to place or remove a tile, depending on if you have an ADD or SUBTRACT layer.
     * ONLY works for ADD and SUBTRACT layers. Does not affect NOMRAL layers.

  3. Available Tiles - A list of WeightTables that determines what possible tiles will be picked on the layer. A Weight Table is a list of Tiles paired to float percentages which act as weights for that tile to be chosen.
     * For every x and y in the map, a weight table is chosen for each layer, and then a tile is chosen FROM that weight table based on the weights.
     * A Tile with a weight of 0.5 will have a 50% chance to be chosen.
     * All weights MUST add up to a total of 1.

  4. Noise Scale - A float that determines how "zoomed" in the noise is for that layer. Larger numbers will be MORE zoomed in.
     * Must be a POSITIVE float
    
  5. Type - An enum that determines the type of noise used for the layer.

  6. Fractal Type - An enum that determines the type of fractal used for the noise.

  7. Fractal Gain - A POSITIVE float that determines the contrast of the nosise.
     * Lower gain will increase contrast (More white and black), higher gain will decrease contrast (More gray values).


See the output folder for examples of outputs.

This generator ONLY works for Unity. Another limitation is that it does not do well with multiple regions/biomes. If you want more biomes, use multiple generators in tandom.
