Algorithm to fit each piece to their own quad, centered on the quad's pivot point:
----------------------------------------------------------------------------------
1. Blit the puzzle once to the quad (the entire puzzle to fit on one quad) during setup
2. Determine the bounds of the visible pixels
3. Set the scale and offset of the quad's material to center the visible area on the quad
	--> IMPORTANT: ensure all pieces are given the same scale as the FIRST piece...or the LARGEST piece (to avoid uv clipping)
4. Attach and configure the bounding boxes for the puzzle piece
5. Leave the quad's material instance alone from then onward.