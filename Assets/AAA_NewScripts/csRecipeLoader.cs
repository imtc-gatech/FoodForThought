using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class csRecipeLoader : MonoBehaviour {

	private csLevelManager levelManager;
	
	void Awake () {
		levelManager = Camera.main.GetComponent<csLevelManager> ();
	}


	public void LoadRecipeXML(string xmlLevelPath) {

		XmlDocument level = new XmlDocument();
		level.Load(xmlLevelPath);
		
		LoadRecipeXML (ref level);

	}


    public void LoadRecipeXML(ref XmlDocument level) {

		if (level.HasChildNodes) {
			XmlNode rootNode = level.FirstChild;

			foreach (XmlAttribute attribute in rootNode.Attributes) {
				switch (attribute.Name) {
				case "title":
					levelManager.levelTitle = attribute.Value;
					break;
				}
			}

			if (rootNode.HasChildNodes) {
				// Child nodes will be individual dishes and primary level data (time limit, customer, etc).
				foreach (XmlNode dishNode in rootNode.ChildNodes) {
					switch (dishNode.Name) {
					case "info":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "author":
								levelManager.levelAuthor = attribute.Value;
								break;
							}
						}
						break;
					case "customer":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "name":
								if (System.Enum.IsDefined (typeof(csCustomer.CustomerName), attribute.Value)) {
									levelManager.levelCustomer = (csCustomer.CustomerName)System.Enum.Parse (typeof(csCustomer.CustomerName), attribute.Value);
								}
								break;
							}
						}
						break;
					case "flavorText":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "text":
								levelManager.levelDescription = attribute.Value;
								break;
							}
						}
						break;
					case "freshness":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "enabled":
								levelManager.freshnessEnabled = bool.Parse (attribute.Value);
								break;
							case "delay":
								levelManager.freshnessDelay = float.Parse (attribute.Value);
								break;
							case "decay":
								levelManager.freshnessDecay = float.Parse (attribute.Value);
								break;
							case "starPenalty":
								levelManager.freshnessMaxPenalty = float.Parse (attribute.Value);
								break;
							}
						}
						break;
					case "timer":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "usesTimer":
								levelManager.isTimedLevel = bool.Parse (attribute.Value);
								break;
							case "timeLimit":
								levelManager.timeLimit = float.Parse (attribute.Value);
								break;
							}
						}
						break;
					case "elapsedTimeTweak":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "multiplier":
										// Is this actually used?  Seems wonky.
                                        //recipeMakerScript.ElapsedTimeMultiplier = Mathf.Clamp(float.Parse(attribute.Value), 0.1f, 10f);
								break;
							}
						}
						break;

					case "stations":
						foreach (XmlAttribute attribute in dishNode.Attributes) {
							int count;

							switch (attribute.Name) {
							case "chop":
								if (System.Int32.TryParse (attribute.Value, out count)) {
									levelManager.kitchen.stations [(int)csStation.StationType.Chop].ConfigureStations (count);
								}
								break;
							case "cook":
								if (System.Int32.TryParse (attribute.Value, out count)) {
									levelManager.kitchen.stations [(int)csStation.StationType.Cook].ConfigureStations (count);
								}
								break;
							case "prep":
								if (System.Int32.TryParse (attribute.Value, out count)) {
									levelManager.kitchen.stations [(int)csStation.StationType.Prep].ConfigureStations (count);
								}
								break;
							case "spice":
								if (System.Int32.TryParse (attribute.Value, out count)) {
									levelManager.kitchen.stations [(int)csStation.StationType.Spice].ConfigureStations (count);
								}
								break;
							}
						}
						break;

					case "dish":
						csDish dish = null;

						foreach (XmlAttribute attribute in dishNode.Attributes) {
							switch (attribute.Name) {
							case "name":
                                        // Name is now the key to loading the correct sprites.
										// Assume it starts with # (and remove that).
								dish = levelManager.counter.AddDish (attribute.Value.Substring (1));
								break;
							}
						}

						if (dishNode.HasChildNodes) {
							// Will contain the steps in this dish.
							int i = 0;

							foreach (XmlNode stepNode in dishNode.ChildNodes) {
								csDishStep step = null;

								if (stepNode.Attributes.GetNamedItem ("destination") != null) {
									if (System.Enum.IsDefined (typeof(csStation.StationType), stepNode.Attributes ["destination"].Value)) {
										step = dish.AddStep ((csStation.StationType)System.Enum.Parse (typeof(csStation.StationType), stepNode.Attributes ["destination"].Value));
									}
								}

								if (stepNode.Attributes.GetNamedItem ("visualState") != null) {
									step.spriteNumber = System.Int32.Parse (stepNode.Attributes ["visualState"].Value);
								} else {
									step.spriteNumber = i;
								}
                                    
								XmlNode gameplayNode = stepNode.FirstChild;
                                    
								step.gameplayType = csDishStep.StepGameplayType.Empty;

								if (System.Enum.IsDefined (typeof(csDishStep.StepGameplayType), gameplayNode.Attributes ["type"].Value)) {
									step.gameplayType = (csDishStep.StepGameplayType)System.Enum.Parse (typeof(csDishStep.StepGameplayType), gameplayNode.Attributes ["type"].Value);
								}

								switch (step.gameplayType) {
								case csDishStep.StepGameplayType.ElapsedTime:
									XmlNode timingParametersNode = gameplayNode.FirstChild;
									XmlNode timingNode = timingParametersNode.FirstChild;
									foreach (XmlNode node in timingNode.ChildNodes) {
										switch (node.Name) {
										case "tweakable":
											foreach (XmlAttribute attribute in node.Attributes) {
												switch (attribute.Name) {
												case "uncooked":
													step.uncookedTime = float.Parse (attribute.Value);
													break;
												case "cooked":
													step.cookedTime = float.Parse (attribute.Value);
													break;
												case "burned":
													step.burnedTime = float.Parse (attribute.Value);
													break;
												case "peak":
													step.peakPercent = System.Int32.Parse (attribute.Value);
													break;
												}
											}
											break;
										case "readonly":
											foreach (XmlAttribute attribute in node.Attributes) {
												switch (attribute.Name) {
												case "isCookable":
													step.isCookable = bool.Parse (attribute.Value);
													break;
												case "isBurnable":
													step.isBurnable = bool.Parse (attribute.Value);
													break;
												case "hasPeakFlavor":
													step.usesPeakFlavor = bool.Parse (attribute.Value);
													break;
												}
											}
											break;
										}
									}

									break;
								case csDishStep.StepGameplayType.MiniGame:
                                            /*
                                            XmlNode minigameParametersNode = gameplayNode.FirstChild;
                                            XmlNode minigameNode = minigameParametersNode.FirstChild;
                                            MG_Minigame.Type gameType = MG_Minigame.MinigameTypeFromString(minigameNode.Name);
                                            currentStep.MinigameType = gameType;
                                            currentStep.MinigameVariableContainer.CurrentType = currentStep.MinigameType;

                                            switch (gameType)
                                            {
												//loadminigameparams
                                                case MG_Minigame.Type.Placeholder:
                                                    foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                    {
                                                        switch (attribute.Name)
                                                        {
                                                            case "delayInSeconds":
                                                                currentStep.MinigameVariableContainer.Placeholder_DelayToWinInSeconds = float.Parse(attribute.Value);
                                                                break;
                                                            case "useDelay":
                                                                currentStep.MinigameVariableContainer.Placeholder_UseDelay = bool.Parse(attribute.Value);
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                case MG_Minigame.Type.Sorting: //TODO: Visual object edit
                                                    foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                    {
                                                        switch (attribute.Name)
                                                        {
                                                            case "numFoodItems":
                                                                currentStep.MinigameVariableContainer.Sorting_NumVegetables = int.Parse(attribute.Value);
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                case MG_Minigame.Type.Stirring: //TODO: Visual object edit
                                                    foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                    {
                                                        switch (attribute.Name)
                                                        {
															case "numVegetables":
                                                                currentStep.MinigameVariableContainer.Stirring_NumberOfVegetables = int.Parse(attribute.Value);
                                                                break;
															case "totalTimeToFinish":
                                                                currentStep.MinigameVariableContainer.Stirring_TotalCookingTime = float.Parse(attribute.Value);
                                                                break;
															case "timeBeforeStir":
																currentStep.MinigameVariableContainer.Stirring_TimeBeforeNeedingToStir = float.Parse(attribute.Value);
																break;
															case "timeBeforeDanger":
																currentStep.MinigameVariableContainer.Stirring_TimeBeforeEnteringDangerZone = float.Parse(attribute.Value);
																break;
															case "timeBeforeBurnt":
																currentStep.MinigameVariableContainer.Stirring_TimeBeforeFullyBurned = float.Parse(attribute.Value);
																break;
															case "burnRecovery":
																currentStep.MinigameVariableContainer.Stirring_BurnRecoveryMultiplier = float.Parse(attribute.Value);
																break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    break;
												case MG_Minigame.Type.Blending: //TODO: More params? Softness? Random ranges?
                                                    foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                    {
                                                        switch (attribute.Name)
                                                        {
                                                            case "numIngredients":
                                                                currentStep.MinigameVariableContainer.Blending_NumIngredients = int.Parse(attribute.Value);
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                case MG_Minigame.Type.Spicing: 
                                                    foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                    {
                                                        switch (attribute.Name)
                                                        {
															case "numBottles":
																currentStep.MinigameVariableContainer.Spicing_NumberOfBottles = int.Parse(attribute.Value);
																break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                case MG_Minigame.Type.Chopping: 
                                                    foreach (XmlAttribute attribute in minigameNode.Attributes)
                                                    {
                                                        switch (attribute.Name)
                                                        {
															case "numCuts":
																currentStep.MinigameVariableContainer.Chopping_MaxCuts = int.Parse(attribute.Value);
																break;
															case "gameMode":
																currentStep.MinigameVariableContainer.Chopping_GameMode = MG2_RootScript.StringToGameMode(attribute.Value);
																break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    break;


                                                    //TODO_INSERT
                                                //insert other cases for other MG_Minigame.Types
                                            }
										*/

											// TODO: Reimplement minigames.

									break;
								case csDishStep.StepGameplayType.Empty:
									break;

								}

								// Done loading this step.  Finalize it.
								step.ConfigureDisplay();

								i++;
							}

							// Done with steps in this dish now.

						}

						// Done with this dish now.
						dish.FinalizeDishPrep();

						break;
					}
				}
			}
		}

		// Now remove unused counter slots.
		levelManager.counter.RemoveUnusedSlots ();
    }
    
}
