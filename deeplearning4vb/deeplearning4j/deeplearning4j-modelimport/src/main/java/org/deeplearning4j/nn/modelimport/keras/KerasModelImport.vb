Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports DL4JFileUtils = org.deeplearning4j.common.util.DL4JFileUtils

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.nn.modelimport.keras


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class KerasModelImport
	Public Class KerasModelImport
		''' <summary>
		''' Load Keras (Functional API) Model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Stream">       InputStream containing HDF5 archive storing Keras Model </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> ComputationGraph </returns>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(InputStream modelHdf5Stream, boolean enforceTrainingConfig) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelHdf5Stream As Stream, ByVal enforceTrainingConfig As Boolean) As ComputationGraph
			Dim f As File = Nothing
			Try
				f = toTempFile(modelHdf5Stream)
				Return importKerasModelAndWeights(f.getAbsolutePath(), enforceTrainingConfig)
			Finally
				If f IsNot Nothing Then
					f.delete()
				End If
			End Try
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Stream"> InputStream containing HDF5 archive storing Keras Model </param>
		''' <returns> ComputationGraph </returns>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(InputStream modelHdf5Stream) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelHdf5Stream As Stream) As ComputationGraph
			Dim f As File = Nothing
			Try
				f = toTempFile(modelHdf5Stream)
				Return importKerasModelAndWeights(f.getAbsolutePath())
			Finally
				If f IsNot Nothing Then
					f.delete()
				End If
			End Try
		End Function

		''' <summary>
		''' Load Keras Sequential model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Stream">       InputStream containing HDF5 archive storing Keras Sequential model </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> ComputationGraph </returns>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(InputStream modelHdf5Stream, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelHdf5Stream As Stream, ByVal enforceTrainingConfig As Boolean) As MultiLayerNetwork
			Dim f As File = Nothing
			Try
				f = toTempFile(modelHdf5Stream)
				Return importKerasSequentialModelAndWeights(f.getAbsolutePath(), enforceTrainingConfig)
			Finally
				If f IsNot Nothing Then
					f.delete()
				End If
			End Try
		End Function

		''' <summary>
		''' Load Keras Sequential model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Stream"> InputStream containing HDF5 archive storing Keras Sequential model </param>
		''' <returns> ComputationGraph </returns>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(InputStream modelHdf5Stream) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelHdf5Stream As Stream) As MultiLayerNetwork
			Dim f As File = Nothing
			Try
				f = toTempFile(modelHdf5Stream)
				Return importKerasSequentialModelAndWeights(f.getAbsolutePath())
			Finally
				If f IsNot Nothing Then
					f.delete()
				End If
			End Try
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Filename">     path to HDF5 archive storing Keras Model </param>
		''' <param name="inputShape">            optional input shape for models that come without such (e.g. notop = false models) </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(String modelHdf5Filename, int[] inputShape, boolean enforceTrainingConfig) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelHdf5Filename As String, ByVal inputShape() As Integer, ByVal enforceTrainingConfig As Boolean) As ComputationGraph
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder_Conflict.modelHdf5Filename(modelHdf5Filename).enforceTrainingConfig(enforceTrainingConfig).inputShape(inputShape).buildModel()
			Return kerasModel_Conflict.ComputationGraph
		End Function


		''' <summary>
		''' Load Keras (Functional API) Model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Filename">     path to HDF5 archive storing Keras Model </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(String modelHdf5Filename, boolean enforceTrainingConfig) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelHdf5Filename As String, ByVal enforceTrainingConfig As Boolean) As ComputationGraph
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder_Conflict.modelHdf5Filename(modelHdf5Filename).enforceTrainingConfig(enforceTrainingConfig).buildModel()
			Return kerasModel_Conflict.ComputationGraph
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Filename"> path to HDF5 archive storing Keras Model </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException">                            IO exception </exception>
		''' <exception cref="InvalidKerasConfigurationException">     Invalid Keras config </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported Keras config </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(String modelHdf5Filename) throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelHdf5Filename As String) As ComputationGraph
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder().modelHdf5Filename(modelHdf5Filename).enforceTrainingConfig(True).buildModel()
			Return kerasModel_Conflict.ComputationGraph
		End Function

		''' <summary>
		''' Load Keras Sequential model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Filename">     path to HDF5 archive storing Keras Sequential model </param>
		''' <param name="inputShape">            optional input shape for models that come without such (e.g. notop = false models) </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(String modelHdf5Filename, int[] inputShape, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelHdf5Filename As String, ByVal inputShape() As Integer, ByVal enforceTrainingConfig As Boolean) As MultiLayerNetwork
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelHdf5Filename(modelHdf5Filename).enforceTrainingConfig(enforceTrainingConfig).inputShape(inputShape).buildSequential()
			Return kerasModel_Conflict.MultiLayerNetwork
		End Function

		''' <summary>
		''' Load Keras Sequential model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Filename">     path to HDF5 archive storing Keras Sequential model </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(String modelHdf5Filename, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelHdf5Filename As String, ByVal enforceTrainingConfig As Boolean) As MultiLayerNetwork
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelHdf5Filename(modelHdf5Filename).enforceTrainingConfig(enforceTrainingConfig).buildSequential()
			Return kerasModel_Conflict.MultiLayerNetwork
		End Function

		''' <summary>
		''' Load Keras Sequential model saved using model.save_model(...).
		''' </summary>
		''' <param name="modelHdf5Filename"> path to HDF5 archive storing Keras Sequential model </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(String modelHdf5Filename) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelHdf5Filename As String) As MultiLayerNetwork
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelHdf5Filename(modelHdf5Filename).enforceTrainingConfig(True).buildSequential()
			Return kerasModel_Conflict.MultiLayerNetwork
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model for which the configuration and weights were
		''' saved separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename">     path to JSON file storing Keras Model configuration </param>
		''' <param name="weightsHdf5Filename">   path to HDF5 archive storing Keras model weights </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(String modelJsonFilename, String weightsHdf5Filename, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelJsonFilename As String, ByVal weightsHdf5Filename As String, ByVal enforceTrainingConfig As Boolean) As ComputationGraph
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder().modelJsonFilename(modelJsonFilename).enforceTrainingConfig(False).weightsHdf5FilenameNoRoot(weightsHdf5Filename).enforceTrainingConfig(enforceTrainingConfig).buildModel()
			Return kerasModel_Conflict.ComputationGraph
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model for which the configuration and weights were
		''' saved separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename">   path to JSON file storing Keras Model configuration </param>
		''' <param name="weightsHdf5Filename"> path to HDF5 archive storing Keras model weights </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph importKerasModelAndWeights(String modelJsonFilename, String weightsHdf5Filename) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasModelAndWeights(ByVal modelJsonFilename As String, ByVal weightsHdf5Filename As String) As ComputationGraph
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder().modelJsonFilename(modelJsonFilename).enforceTrainingConfig(False).weightsHdf5FilenameNoRoot(weightsHdf5Filename).enforceTrainingConfig(True).buildModel()
			Return kerasModel_Conflict.ComputationGraph
		End Function

		''' <summary>
		''' Load Keras Sequential model for which the configuration and weights were
		''' saved separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename">     path to JSON file storing Keras Sequential model configuration </param>
		''' <param name="weightsHdf5Filename">   path to HDF5 archive storing Keras model weights </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(String modelJsonFilename, String weightsHdf5Filename, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelJsonFilename As String, ByVal weightsHdf5Filename As String, ByVal enforceTrainingConfig As Boolean) As MultiLayerNetwork
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelJsonFilename(modelJsonFilename).weightsHdf5FilenameNoRoot(weightsHdf5Filename).enforceTrainingConfig(enforceTrainingConfig).buildSequential()
			Return kerasModel_Conflict.MultiLayerNetwork
		End Function

		''' <summary>
		''' Load Keras Sequential model for which the configuration and weights were
		''' saved separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename">   path to JSON file storing Keras Sequential model configuration </param>
		''' <param name="weightsHdf5Filename"> path to HDF5 archive storing Keras model weights </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork importKerasSequentialModelAndWeights(String modelJsonFilename, String weightsHdf5Filename) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialModelAndWeights(ByVal modelJsonFilename As String, ByVal weightsHdf5Filename As String) As MultiLayerNetwork
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelJsonFilename(modelJsonFilename).weightsHdf5FilenameNoRoot(weightsHdf5Filename).enforceTrainingConfig(False).buildSequential()
			Return kerasModel_Conflict.MultiLayerNetwork
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model for which the configuration was saved
		''' separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename">     path to JSON file storing Keras Model configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.ComputationGraphConfiguration importKerasModelConfiguration(String modelJsonFilename, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasModelConfiguration(ByVal modelJsonFilename As String, ByVal enforceTrainingConfig As Boolean) As ComputationGraphConfiguration
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder().modelJsonFilename(modelJsonFilename).enforceTrainingConfig(enforceTrainingConfig).buildModel()
			Return kerasModel_Conflict.ComputationGraphConfiguration
		End Function

		''' <summary>
		''' Load Keras (Functional API) Model for which the configuration was saved
		''' separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename"> path to JSON file storing Keras Model configuration </param>
		''' <returns> ComputationGraph </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= ComputationGraph </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.ComputationGraphConfiguration importKerasModelConfiguration(String modelJsonFilename) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasModelConfiguration(ByVal modelJsonFilename As String) As ComputationGraphConfiguration
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasModel = (New KerasModel()).modelBuilder().modelJsonFilename(modelJsonFilename).enforceTrainingConfig(False).buildModel()
			Return kerasModel_Conflict.ComputationGraphConfiguration
		End Function

		''' <summary>
		''' Load Keras Sequential model for which the configuration was saved
		''' separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename">     path to JSON file storing Keras Sequential model configuration </param>
		''' <param name="enforceTrainingConfig"> whether to enforce training configuration options </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.MultiLayerConfiguration importKerasSequentialConfiguration(String modelJsonFilename, boolean enforceTrainingConfig) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialConfiguration(ByVal modelJsonFilename As String, ByVal enforceTrainingConfig As Boolean) As MultiLayerConfiguration
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelJsonFilename(modelJsonFilename).enforceTrainingConfig(enforceTrainingConfig).buildSequential()
			Return kerasModel_Conflict.MultiLayerConfiguration
		End Function

		''' <summary>
		''' Load Keras Sequential model for which the configuration was saved
		''' separately using calls to model.to_json() and model.save_weights(...).
		''' </summary>
		''' <param name="modelJsonFilename"> path to JSON file storing Keras Sequential model configuration </param>
		''' <returns> MultiLayerNetwork </returns>
		''' <exception cref="IOException"> IO exception </exception>
		''' <seealso cref= MultiLayerNetwork </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.conf.MultiLayerConfiguration importKerasSequentialConfiguration(String modelJsonFilename) throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Shared Function importKerasSequentialConfiguration(ByVal modelJsonFilename As String) As MultiLayerConfiguration
'JAVA TO VB CONVERTER NOTE: The variable kerasModel was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim kerasModel_Conflict As KerasSequentialModel = (New KerasSequentialModel()).modelBuilder().modelJsonFilename(modelJsonFilename).enforceTrainingConfig(False).buildSequential()
			Return kerasModel_Conflict.MultiLayerConfiguration
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static File toTempFile(InputStream is) throws IOException
		Private Shared Function toTempFile(ByVal [is] As Stream) As File
			Dim f As File = DL4JFileUtils.createTempFile("DL4JKerasModelImport",".bin")
			f.deleteOnExit()


			Using os As Stream = New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
				IOUtils.copy([is], os)
				os.Flush()
				Return f
			End Using
		End Function
	End Class

End Namespace