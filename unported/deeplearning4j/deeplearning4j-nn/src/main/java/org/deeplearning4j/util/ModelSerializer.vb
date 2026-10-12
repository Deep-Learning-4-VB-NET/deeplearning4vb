Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports CloseShieldInputStream = org.apache.commons.io.input.CloseShieldInputStream
Imports DL4JFileUtils = org.deeplearning4j.common.util.DL4JFileUtils
Imports Files = org.nd4j.shade.guava.io.Files
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports CloseShieldOutputStream = org.apache.commons.io.output.CloseShieldOutputStream
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports Model = org.deeplearning4j.nn.api.Model
Imports Updater = org.deeplearning4j.nn.api.Updater
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization
Imports org.nd4j.linalg.dataset.api.preprocessor
Imports NormalizerSerializer = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerSerializer
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Task = org.nd4j.linalg.heartbeat.reports.Task
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ModelSerializer
	Public Class ModelSerializer

		Public Const UPDATER_BIN As String = "updaterState.bin"
		Public Const NORMALIZER_BIN As String = "normalizer.bin"
		Public Const CONFIGURATION_JSON As String = "configuration.json"
		Public Const COEFFICIENTS_BIN As String = "coefficients.bin"
		Public Const NO_PARAMS_MARKER As String = "noParams.marker"
		Public Const PREPROCESSOR_BIN As String = "preprocessor.bin"

		Private Sub New()
		End Sub

		''' <summary>
		''' Write a model to a file </summary>
		''' <param name="model"> the model to write </param>
		''' <param name="file"> the file to write to </param>
		''' <param name="saveUpdater"> whether to save the updater or not </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeModel(@NonNull Model model, @NonNull File file, boolean saveUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeModel(ByVal model As Model, ByVal file As File, ByVal saveUpdater As Boolean)
			writeModel(model,file,saveUpdater,Nothing)
		End Sub



		''' <summary>
		''' Write a model to a file </summary>
		''' <param name="model"> the model to write </param>
		''' <param name="file"> the file to write to </param>
		''' <param name="saveUpdater"> whether to save the updater or not </param>
		''' <param name="dataNormalization"> the normalizer to save (optional) </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeModel(@NonNull Model model, @NonNull File file, boolean saveUpdater,org.nd4j.linalg.dataset.api.preprocessor.DataNormalization dataNormalization) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeModel(ByVal model As Model, ByVal file As File, ByVal saveUpdater As Boolean, ByVal dataNormalization As DataNormalization)
			Using stream As New BufferedOutputStream(New FileStream(file, FileMode.Create, FileAccess.Write))
				writeModel(model, stream, saveUpdater,dataNormalization)
			End Using
		End Sub


		''' <summary>
		''' Write a model to a file path </summary>
		''' <param name="model"> the model to write </param>
		''' <param name="path"> the path to write to </param>
		''' <param name="saveUpdater"> whether to save the updater
		'''                    or not </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeModel(@NonNull Model model, @NonNull String path, boolean saveUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeModel(ByVal model As Model, ByVal path As String, ByVal saveUpdater As Boolean)
			Using stream As New BufferedOutputStream(New FileStream(path, FileMode.Create, FileAccess.Write))
				writeModel(model, stream, saveUpdater)
			End Using
		End Sub

		''' <summary>
		''' Write a model to an output stream </summary>
		''' <param name="model"> the model to save </param>
		''' <param name="stream"> the output stream to write to </param>
		''' <param name="saveUpdater"> whether to save the updater for the model or not </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeModel(@NonNull Model model, @NonNull OutputStream stream, boolean saveUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeModel(ByVal model As Model, ByVal stream As Stream, ByVal saveUpdater As Boolean)
			writeModel(model,stream,saveUpdater,Nothing)
		End Sub




		''' <summary>
		''' Write a model to an output stream </summary>
		''' <param name="model"> the model to save </param>
		''' <param name="stream"> the output stream to write to </param>
		''' <param name="saveUpdater"> whether to save the updater for the model or not </param>
		''' <param name="dataNormalization"> the normalizer ot save (may be null) </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void writeModel(@NonNull Model model, @NonNull OutputStream stream, boolean saveUpdater,org.nd4j.linalg.dataset.api.preprocessor.DataNormalization dataNormalization) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub writeModel(ByVal model As Model, ByVal stream As Stream, ByVal saveUpdater As Boolean, ByVal dataNormalization As DataNormalization)
			Dim zipfile As New ZipOutputStream(New CloseShieldOutputStream(stream))

			' Save configuration as JSON
			Dim json As String = ""
			If TypeOf model Is MultiLayerNetwork Then
				json = CType(model, MultiLayerNetwork).LayerWiseConfigurations.toJson()
			ElseIf TypeOf model Is ComputationGraph Then
				json = CType(model, ComputationGraph).Configuration.toJson()
			End If

			Dim config As New ZipEntry(CONFIGURATION_JSON)
			zipfile.putNextEntry(config)
			zipfile.write(json.GetBytes())

			' Save parameters as binary
			Dim coefficients As New ZipEntry(COEFFICIENTS_BIN)
			zipfile.putNextEntry(coefficients)
			Dim dos As New DataOutputStream(New BufferedOutputStream(zipfile))
			Dim params As INDArray = model.params()
			If params IsNot Nothing Then
				Try
					Nd4j.write(model.params(), dos)
				Finally
					dos.flush()
				End Try
			Else
				Dim noParamsMarker As New ZipEntry(NO_PARAMS_MARKER)
				zipfile.putNextEntry(noParamsMarker)
			End If

			If saveUpdater Then
				Dim updaterState As INDArray = Nothing
				If TypeOf model Is MultiLayerNetwork Then
					updaterState = CType(model, MultiLayerNetwork).Updater.StateViewArray
				ElseIf TypeOf model Is ComputationGraph Then
					updaterState = CType(model, ComputationGraph).Updater.StateViewArray
				End If

				If updaterState IsNot Nothing AndAlso updaterState.length() > 0 Then
					Dim updater As New ZipEntry(UPDATER_BIN)
					zipfile.putNextEntry(updater)

					Try
						Nd4j.write(updaterState, dos)
					Finally
						dos.flush()
					End Try
				End If
			End If


			If dataNormalization IsNot Nothing Then
				' now, add our normalizer as additional entry
				Dim nEntry As New ZipEntry(NORMALIZER_BIN)
				zipfile.putNextEntry(nEntry)
				NormalizerSerializer.Default.write(dataNormalization, zipfile)
			End If

			dos.close()
			zipfile.close()
		End Sub

		''' <summary>
		''' Load a multi layer network from a file
		''' </summary>
		''' <param name="file"> the file to load from </param>
		''' <returns> the loaded multi layer network </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork restoreMultiLayerNetwork(@NonNull File file) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetwork(ByVal file As File) As MultiLayerNetwork
			Return restoreMultiLayerNetwork(file, True)
		End Function


		''' <summary>
		''' Load a multi layer network from a file
		''' </summary>
		''' <param name="file"> the file to load from </param>
		''' <returns> the loaded multi layer network </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork restoreMultiLayerNetwork(@NonNull File file, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetwork(ByVal file As File, ByVal loadUpdater As Boolean) As MultiLayerNetwork
			Using [is] As Stream = New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
				Return restoreMultiLayerNetwork([is], loadUpdater)
			End Using
		End Function


		''' <summary>
		''' Load a MultiLayerNetwork from InputStream from an input stream<br>
		''' Note: the input stream is read fully and closed by this method. Consequently, the input stream cannot be re-used.
		''' </summary>
		''' <param name="is"> the inputstream to load from </param>
		''' <returns> the loaded multi layer network </returns>
		''' <exception cref="IOException"> </exception>
		''' <seealso cref= #restoreMultiLayerNetworkAndNormalizer(InputStream, boolean) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork restoreMultiLayerNetwork(@NonNull InputStream is, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetwork(ByVal [is] As Stream, ByVal loadUpdater As Boolean) As MultiLayerNetwork
			Return restoreMultiLayerNetworkHelper([is], loadUpdater).First
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.multilayer.MultiLayerNetwork, java.util.Map<String,byte[]>> restoreMultiLayerNetworkHelper(@NonNull InputStream is, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Private Shared Function restoreMultiLayerNetworkHelper(ByVal [is] As Stream, ByVal loadUpdater As Boolean) As Pair(Of MultiLayerNetwork, IDictionary(Of String, SByte()))
			checkInputStream([is])

			Dim zipFile As IDictionary(Of String, SByte()) = loadZipData([is])

			Dim gotConfig As Boolean = False
			Dim gotCoefficients As Boolean = False
			Dim gotUpdaterState As Boolean = False
			Dim gotPreProcessor As Boolean = False

			Dim json As String = ""
			Dim params As INDArray = Nothing
			Dim updater As Updater = Nothing
			Dim updaterState As INDArray = Nothing
			Dim preProcessor As DataSetPreProcessor = Nothing


			Dim config() As SByte = zipFile(CONFIGURATION_JSON)
			If config IsNot Nothing Then
				'restoring configuration

				Dim stream As Stream = New MemoryStream(config)
				Dim reader As New StreamReader(stream)
				Dim line As String = ""
				Dim js As New StringBuilder()
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					js.Append(line).Append(vbLf)
						line = reader.ReadLine()
				Loop
				json = js.ToString()

				reader.Close()
				stream.Close()
				gotConfig = True
			End If


			Dim coefficients() As SByte = zipFile(COEFFICIENTS_BIN)
			If coefficients IsNot Nothing Then
				If coefficients.Length > 0 Then
					Dim stream As Stream = New MemoryStream(coefficients)
					Dim dis As New DataInputStream(New BufferedInputStream(stream))
					params = Nd4j.read(dis)

					dis.close()
					gotCoefficients = True
				Else
					Dim noParamsMarker() As SByte = zipFile(NO_PARAMS_MARKER)
					gotCoefficients = (noParamsMarker IsNot Nothing)
				End If
			End If

			If loadUpdater Then
				Dim updaterStateEntry() As SByte = zipFile(UPDATER_BIN)
				If updaterStateEntry IsNot Nothing Then
					Dim stream As Stream = New MemoryStream(updaterStateEntry)
					Dim dis As New DataInputStream(New BufferedInputStream(stream))
					updaterState = Nd4j.read(dis)

					dis.close()
					gotUpdaterState = True
				End If
			End If

			Dim prep() As SByte = zipFile(PREPROCESSOR_BIN)
			If prep IsNot Nothing Then
				Dim stream As Stream = New MemoryStream(prep)
				Dim ois As New ObjectInputStream(stream)

				Try
					preProcessor = DirectCast(ois.readObject(), DataSetPreProcessor)
				Catch e As ClassNotFoundException
					Throw New Exception(e)
				End Try

				gotPreProcessor = True
			End If



			If gotConfig AndAlso gotCoefficients Then
				Dim confFromJson As MultiLayerConfiguration
				Try
				   confFromJson = MultiLayerConfiguration.fromJson(json)
				Catch e As Exception
					Dim cg As ComputationGraphConfiguration
					Try
						cg = ComputationGraphConfiguration.fromJson(json)
					Catch e2 As Exception
						'Invalid, and not a compgraph
						Throw New Exception("Error deserializing JSON MultiLayerConfiguration. Saved model JSON is" & " not a valid MultiLayerConfiguration", e)
					End Try
					If cg.getNetworkInputs() IsNot Nothing AndAlso cg.getVertices() IsNot Nothing Then
						Throw New Exception("Error deserializing JSON MultiLayerConfiguration. Saved model appears to be " & "a ComputationGraph - use ModelSerializer.restoreComputationGraph instead")
					Else
						Throw e
					End If
				End Try

				'Handle legacy config - no network DataType in config, in beta3 or earlier
				If params IsNot Nothing Then
					confFromJson.setDataType(params.dataType())
				End If
				Dim network As New MultiLayerNetwork(confFromJson)
				network.init(params, False)

				If gotUpdaterState AndAlso updaterState IsNot Nothing Then
					network.Updater.setStateViewArray(network, updaterState, False)
				End If
				Return New Pair(Of MultiLayerNetwork, IDictionary(Of String, SByte()))(network, zipFile)
			Else
				Throw New System.InvalidOperationException("Model wasnt found within file: gotConfig: [" & gotConfig & "], gotCoefficients: [" & gotCoefficients & "], gotUpdater: [" & gotUpdaterState & "]")
			End If
		End Function

		''' <summary>
		''' Restore a multi layer network from an input stream<br>
		''' * Note: the input stream is read fully and closed by this method. Consequently, the input stream cannot be re-used.
		''' 
		''' </summary>
		''' <param name="is"> the input stream to restore from </param>
		''' <returns> the loaded multi layer network </returns>
		''' <exception cref="IOException"> </exception>
		''' <seealso cref= #restoreMultiLayerNetworkAndNormalizer(InputStream, boolean) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork restoreMultiLayerNetwork(@NonNull InputStream is) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetwork(ByVal [is] As Stream) As MultiLayerNetwork
			Return restoreMultiLayerNetwork([is], True)
		End Function

		''' <summary>
		''' Load a MultilayerNetwork model from a file
		''' </summary>
		''' <param name="path"> path to the model file, to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork restoreMultiLayerNetwork(@NonNull String path) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetwork(ByVal path As String) As MultiLayerNetwork
			Return restoreMultiLayerNetwork(New File(path), True)
		End Function

		''' <summary>
		''' Load a MultilayerNetwork model from a file </summary>
		''' <param name="path"> path to the model file, to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.multilayer.MultiLayerNetwork restoreMultiLayerNetwork(@NonNull String path, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetwork(ByVal path As String, ByVal loadUpdater As Boolean) As MultiLayerNetwork
			Return restoreMultiLayerNetwork(New File(path), loadUpdater)
		End Function

		''' <summary>
		''' Restore a MultiLayerNetwork and Normalizer (if present - null if not) from the InputStream.
		''' Note: the input stream is read fully and closed by this method. Consequently, the input stream cannot be re-used.
		''' </summary>
		''' <param name="is">          Input stream to read from </param>
		''' <param name="loadUpdater"> Whether to load the updater from the model or not </param>
		''' <returns> Model and normalizer, if present </returns>
		''' <exception cref="IOException"> If an error occurs when reading from the stream </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.multilayer.MultiLayerNetwork, org.nd4j.linalg.dataset.api.preprocessor.Normalizer> restoreMultiLayerNetworkAndNormalizer(@NonNull InputStream is, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetworkAndNormalizer(ByVal [is] As Stream, ByVal loadUpdater As Boolean) As Pair(Of MultiLayerNetwork, Normalizer)
			checkInputStream([is])
			[is] = New CloseShieldInputStream([is])

			Dim p As Pair(Of MultiLayerNetwork, IDictionary(Of String, SByte())) = restoreMultiLayerNetworkHelper([is], loadUpdater)
			Dim net As MultiLayerNetwork = p.First
			Dim norm As Normalizer = restoreNormalizerFromMap(p.Second)
			Return New Pair(Of MultiLayerNetwork, Normalizer)(net, norm)
		End Function

		''' <summary>
		''' Restore a MultiLayerNetwork and Normalizer (if present - null if not) from a File
		''' </summary>
		''' <param name="file">        File to read the model and normalizer from </param>
		''' <param name="loadUpdater"> Whether to load the updater from the model or not </param>
		''' <returns> Model and normalizer, if present </returns>
		''' <exception cref="IOException"> If an error occurs when reading from the File </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.multilayer.MultiLayerNetwork, org.nd4j.linalg.dataset.api.preprocessor.Normalizer> restoreMultiLayerNetworkAndNormalizer(@NonNull File file, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreMultiLayerNetworkAndNormalizer(ByVal file As File, ByVal loadUpdater As Boolean) As Pair(Of MultiLayerNetwork, Normalizer)
			Using [is] As Stream = New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
				Return restoreMultiLayerNetworkAndNormalizer([is], loadUpdater)
			End Using
		End Function

		''' <summary>
		''' Load a computation graph from a file </summary>
		''' <param name="path"> path to the model file, to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph restoreComputationGraph(@NonNull String path) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraph(ByVal path As String) As ComputationGraph
			Return restoreComputationGraph(New File(path), True)
		End Function

		''' <summary>
		''' Load a computation graph from a file </summary>
		''' <param name="path"> path to the model file, to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph restoreComputationGraph(@NonNull String path, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraph(ByVal path As String, ByVal loadUpdater As Boolean) As ComputationGraph
			Return restoreComputationGraph(New File(path), loadUpdater)
		End Function


		''' <summary>
		''' Load a computation graph from a InputStream </summary>
		''' <param name="is"> the inputstream to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph restoreComputationGraph(@NonNull InputStream is, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraph(ByVal [is] As Stream, ByVal loadUpdater As Boolean) As ComputationGraph
			Return restoreComputationGraphHelper([is], loadUpdater).First
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.graph.ComputationGraph,java.util.Map<String,byte[]>> restoreComputationGraphHelper(@NonNull InputStream is, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Private Shared Function restoreComputationGraphHelper(ByVal [is] As Stream, ByVal loadUpdater As Boolean) As Pair(Of ComputationGraph, IDictionary(Of String, SByte()))
			checkInputStream([is])

			Dim files As IDictionary(Of String, SByte()) = loadZipData([is])

			Dim gotConfig As Boolean = False
			Dim gotCoefficients As Boolean = False
			Dim gotUpdaterState As Boolean = False
			Dim gotPreProcessor As Boolean = False

			Dim json As String = ""
			Dim params As INDArray = Nothing
			Dim updaterState As INDArray = Nothing
			Dim preProcessor As DataSetPreProcessor = Nothing


			Dim config() As SByte = files(CONFIGURATION_JSON)
			If config IsNot Nothing Then
				'restoring configuration

				Dim stream As Stream = New MemoryStream(config)
				Dim reader As New StreamReader(stream)
				Dim line As String = ""
				Dim js As New StringBuilder()
				line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
				Do While line IsNot Nothing
					js.Append(line).Append(vbLf)
						line = reader.ReadLine()
				Loop
				json = js.ToString()

				reader.Close()
				stream.Close()
				gotConfig = True
			End If


			Dim coefficients() As SByte = files(COEFFICIENTS_BIN)
			If coefficients IsNot Nothing Then
				If coefficients.Length > 0 Then
					Dim stream As Stream = New MemoryStream(coefficients)
					Dim dis As New DataInputStream(stream)
					params = Nd4j.read(dis)

					dis.close()
					gotCoefficients = True
				Else
					Dim noParamsMarker() As SByte = files(NO_PARAMS_MARKER)
					gotCoefficients = (noParamsMarker IsNot Nothing)
				End If
			End If


			If loadUpdater Then
				Dim updaterStateEntry() As SByte = files(UPDATER_BIN)
				If updaterStateEntry IsNot Nothing Then
					Dim stream As Stream = New MemoryStream(updaterStateEntry)
					Dim dis As New DataInputStream(stream)
					updaterState = Nd4j.read(dis)

					dis.close()
					gotUpdaterState = True
				End If
			End If

			Dim prep() As SByte = files(PREPROCESSOR_BIN)
			If prep IsNot Nothing Then
				Dim stream As Stream = New MemoryStream(prep)
				Dim ois As New ObjectInputStream(stream)

				Try
					preProcessor = DirectCast(ois.readObject(), DataSetPreProcessor)
				Catch e As ClassNotFoundException
					Throw New Exception(e)
				End Try

				gotPreProcessor = True
			End If


			If gotConfig AndAlso gotCoefficients Then
				Dim confFromJson As ComputationGraphConfiguration
				Try
					confFromJson = ComputationGraphConfiguration.fromJson(json)
					If confFromJson.getNetworkInputs() Is Nothing AndAlso (confFromJson.getVertices() Is Nothing OrElse confFromJson.getVertices().size() = 0) Then
						'May be deserialized correctly, but mostly with null fields
						Throw New Exception("Invalid JSON - not a ComputationGraphConfiguration")
					End If
				Catch e As Exception
					If e.Message IsNot Nothing AndAlso e.Message.contains("registerLegacyCustomClassesForJSON") Then
						Throw e
					End If
					Try
						MultiLayerConfiguration.fromJson(json)
					Catch e2 As Exception
						'Invalid, and not a compgraph
						Throw New Exception("Error deserializing JSON ComputationGraphConfiguration. Saved model JSON is" & " not a valid ComputationGraphConfiguration", e)
					End Try
					Throw New Exception("Error deserializing JSON ComputationGraphConfiguration. Saved model appears to be " & "a MultiLayerNetwork - use ModelSerializer.restoreMultiLayerNetwork instead")
				End Try

				'Handle legacy config - no network DataType in config, in beta3 or earlier
				If params IsNot Nothing Then
					confFromJson.setDataType(params.dataType())
				End If

				Dim cg As New ComputationGraph(confFromJson)
				cg.init(params, False)


				If gotUpdaterState AndAlso updaterState IsNot Nothing Then
					cg.Updater.StateViewArray = updaterState
				End If
				Return New Pair(Of ComputationGraph, IDictionary(Of String, SByte()))(cg, files)
			Else
				Throw New System.InvalidOperationException("Model wasnt found within file: gotConfig: [" & gotConfig & "], gotCoefficients: [" & gotCoefficients & "], gotUpdater: [" & gotUpdaterState & "]")
			End If
		End Function

		''' <summary>
		''' Load a computation graph from a InputStream </summary>
		''' <param name="is"> the inputstream to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph restoreComputationGraph(@NonNull InputStream is) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraph(ByVal [is] As Stream) As ComputationGraph
			Return restoreComputationGraph([is], True)
		End Function

		''' <summary>
		''' Load a computation graph from a file </summary>
		''' <param name="file"> the file to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph restoreComputationGraph(@NonNull File file) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraph(ByVal file As File) As ComputationGraph
			Return restoreComputationGraph(file, True)
		End Function

		''' <summary>
		''' Restore a ComputationGraph and Normalizer (if present - null if not) from the InputStream.
		''' Note: the input stream is read fully and closed by this method. Consequently, the input stream cannot be re-used.
		''' </summary>
		''' <param name="is">          Input stream to read from </param>
		''' <param name="loadUpdater"> Whether to load the updater from the model or not </param>
		''' <returns> Model and normalizer, if present </returns>
		''' <exception cref="IOException"> If an error occurs when reading from the stream </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.graph.ComputationGraph, org.nd4j.linalg.dataset.api.preprocessor.Normalizer> restoreComputationGraphAndNormalizer(@NonNull InputStream is, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraphAndNormalizer(ByVal [is] As Stream, ByVal loadUpdater As Boolean) As Pair(Of ComputationGraph, Normalizer)
			checkInputStream([is])


			Dim p As Pair(Of ComputationGraph, IDictionary(Of String, SByte())) = restoreComputationGraphHelper([is], loadUpdater)
			Dim net As ComputationGraph = p.First
			Dim norm As Normalizer = restoreNormalizerFromMap(p.Second)
			Return New Pair(Of ComputationGraph, Normalizer)(net, norm)
		End Function

		''' <summary>
		''' Restore a ComputationGraph and Normalizer (if present - null if not) from a File
		''' </summary>
		''' <param name="file">        File to read the model and normalizer from </param>
		''' <param name="loadUpdater"> Whether to load the updater from the model or not </param>
		''' <returns> Model and normalizer, if present </returns>
		''' <exception cref="IOException"> If an error occurs when reading from the File </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.graph.ComputationGraph, org.nd4j.linalg.dataset.api.preprocessor.Normalizer> restoreComputationGraphAndNormalizer(@NonNull File file, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraphAndNormalizer(ByVal file As File, ByVal loadUpdater As Boolean) As Pair(Of ComputationGraph, Normalizer)
			Return restoreComputationGraphAndNormalizer(New FileStream(file, FileMode.Open, FileAccess.Read), loadUpdater)
		End Function

		''' <summary>
		''' Load a computation graph from a file </summary>
		''' <param name="file"> the file to get the computation graph from </param>
		''' <returns> the loaded computation graph
		''' </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.deeplearning4j.nn.graph.ComputationGraph restoreComputationGraph(@NonNull File file, boolean loadUpdater) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function restoreComputationGraph(ByVal file As File, ByVal loadUpdater As Boolean) As ComputationGraph
			Return restoreComputationGraph(New FileStream(file, FileMode.Open, FileAccess.Read), loadUpdater)
		End Function

		''' 
		''' <param name="model">
		''' @return </param>
		Public Shared Function taskByModel(ByVal model As Model) As Task
			Dim task As New Task()
			Try
				task.setArchitectureType(Task.ArchitectureType.RECURRENT)
				If TypeOf model Is ComputationGraph Then
					task.setNetworkType(Task.NetworkType.ComputationalGraph)
					Dim network As ComputationGraph = DirectCast(model, ComputationGraph)
					Try
						If network.Layers IsNot Nothing AndAlso network.Layers.Length > 0 Then
							For Each layer As Layer In network.Layers
								If layer.type().Equals(Layer.Type.CONVOLUTIONAL) Then
									task.setArchitectureType(Task.ArchitectureType.CONVOLUTION)
									Exit For
								ElseIf layer.type().Equals(Layer.Type.RECURRENT) OrElse layer.type().Equals(Layer.Type.RECURSIVE) Then
									task.setArchitectureType(Task.ArchitectureType.RECURRENT)
									Exit For
								End If
							Next layer
						Else
							task.setArchitectureType(Task.ArchitectureType.UNKNOWN)
						End If
					Catch e As Exception
						' do nothing here
					End Try
				ElseIf TypeOf model Is MultiLayerNetwork Then
					task.setNetworkType(Task.NetworkType.MultilayerNetwork)
					Dim network As MultiLayerNetwork = DirectCast(model, MultiLayerNetwork)
					Try
						If network.Layers IsNot Nothing AndAlso network.Layers.Length > 0 Then
							For Each layer As Layer In network.Layers
								If layer.type().Equals(Layer.Type.CONVOLUTIONAL) Then
									task.setArchitectureType(Task.ArchitectureType.CONVOLUTION)
									Exit For
								ElseIf layer.type().Equals(Layer.Type.RECURRENT) OrElse layer.type().Equals(Layer.Type.RECURSIVE) Then
									task.setArchitectureType(Task.ArchitectureType.RECURRENT)
									Exit For
								End If
							Next layer
						Else
							task.setArchitectureType(Task.ArchitectureType.UNKNOWN)
						End If
					Catch e As Exception
						' do nothing here
					End Try
				End If
				Return task
			Catch e As Exception
				task.setArchitectureType(Task.ArchitectureType.UNKNOWN)
				task.setNetworkType(Task.NetworkType.DenseNetwork)
				Return task
			End Try
		End Function

		''' <summary>
		''' This method appends normalizer to a given persisted model.
		''' 
		''' PLEASE NOTE: File should be model file saved earlier with ModelSerializer
		''' </summary>
		''' <param name="f"> </param>
		''' <param name="normalizer"> </param>
		Public Shared Sub addNormalizerToModel(Of T1)(ByVal f As File, ByVal normalizer As Normalizer(Of T1))
			Dim tempFile As File = Nothing
			Try
				' copy existing model to temporary file
				tempFile = DL4JFileUtils.createTempFile("dl4jModelSerializerTemp", "bin")
				tempFile.deleteOnExit()
				Files.copy(f, tempFile)
				Using zipFile As New java.util.zip.ZipFile(tempFile), writeFile As New java.util.zip.ZipOutputStream(New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write)))
					' roll over existing files within model, and copy them one by one
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> entries = zipFile.entries();
					Dim entries As IEnumerator(Of ZipEntry) = zipFile.entries()
					Do While entries.MoveNext()
						Dim entry As ZipEntry = entries.Current

						' we're NOT copying existing normalizer, if any
						If entry.getName().equalsIgnoreCase(NORMALIZER_BIN) Then
							Continue Do
						End If

						log.debug("Copying: {}", entry.getName())

						Dim [is] As Stream = zipFile.getInputStream(entry)

						Dim wEntry As New ZipEntry(entry.getName())
						writeFile.putNextEntry(wEntry)

						IOUtils.copy([is], writeFile)
					Loop
					' now, add our normalizer as additional entry
					Dim nEntry As New ZipEntry(NORMALIZER_BIN)
					writeFile.putNextEntry(nEntry)

					NormalizerSerializer.Default.write(normalizer, writeFile)
				End Using
			Catch ex As Exception
				Throw New Exception(ex)
			Finally
				If tempFile IsNot Nothing Then
					tempFile.delete()
				End If
			End Try
		End Sub

		''' <summary>
		''' Add an object to the (already existing) model file using Java Object Serialization. Objects can be restored
		''' using <seealso cref="getObjectFromFile(File, String)"/> </summary>
		''' <param name="f">   File to add the object to </param>
		''' <param name="key"> Key to store the object under </param>
		''' <param name="o">   Object to store using Java object serialization </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void addObjectToFile(@NonNull File f, @NonNull String key, @NonNull Object o)
		Public Shared Sub addObjectToFile(ByVal f As File, ByVal key As String, ByVal o As Object)
			Preconditions.checkState(f.exists(), "File must exist: %s", f)
			Preconditions.checkArgument(Not (UPDATER_BIN.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse NORMALIZER_BIN.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse CONFIGURATION_JSON.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse COEFFICIENTS_BIN.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse NO_PARAMS_MARKER.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse PREPROCESSOR_BIN.Equals(key, StringComparison.OrdinalIgnoreCase)), "Invalid key: Key is reserved for internal use: ""%s""", key)
			Dim tempFile As File = Nothing
			Try
				' copy existing model to temporary file
				tempFile = DL4JFileUtils.createTempFile("dl4jModelSerializerTemp", "bin")
				Files.copy(f, tempFile)
				f.delete()
				Using zipFile As New java.util.zip.ZipFile(tempFile), writeFile As New java.util.zip.ZipOutputStream(New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write)))
					' roll over existing files within model, and copy them one by one
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> entries = zipFile.entries();
					Dim entries As IEnumerator(Of ZipEntry) = zipFile.entries()
					Do While entries.MoveNext()
						Dim entry As ZipEntry = entries.Current

						log.debug("Copying: {}", entry.getName())

						Dim [is] As Stream = zipFile.getInputStream(entry)

						Dim wEntry As New ZipEntry(entry.getName())
						writeFile.putNextEntry(wEntry)

						IOUtils.copy([is], writeFile)
						writeFile.closeEntry()
						[is].Close()
					Loop

					'Add new object:

					Using baos As New MemoryStream(), oos As New ObjectOutputStream(baos)
						oos.writeObject(o)
						Dim bytes() As SByte = baos.toByteArray()
						Dim entry As New ZipEntry("objects/" & key)
						entry.setSize(bytes.Length)
						writeFile.putNextEntry(entry)
						writeFile.write(bytes)
						writeFile.closeEntry()
					End Using

					writeFile.close()
					zipFile.close()

				End Using
			Catch ex As Exception
				Throw New Exception(ex)
			Finally
				If tempFile IsNot Nothing Then
					tempFile.delete()
				End If
			End Try
		End Sub

		''' <summary>
		''' Get an object with the specified key from the model file, that was previously added to the file using
		''' <seealso cref="addObjectToFile(File, String, Object)"/>
		''' </summary>
		''' <param name="f">   model file to add the object to </param>
		''' <param name="key"> Key for the object </param>
		''' @param <T> Type of the object </param>
		''' <returns> The serialized object </returns>
		''' <seealso cref= #listObjectsInFile(File) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static <T> T getObjectFromFile(@NonNull File f, @NonNull String key)
		Public Shared Function getObjectFromFile(Of T)(ByVal f As File, ByVal key As String) As T
			Preconditions.checkState(f.exists(), "File must exist: %s", f)
			Preconditions.checkArgument(Not (UPDATER_BIN.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse NORMALIZER_BIN.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse CONFIGURATION_JSON.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse COEFFICIENTS_BIN.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse NO_PARAMS_MARKER.Equals(key, StringComparison.OrdinalIgnoreCase) OrElse PREPROCESSOR_BIN.Equals(key, StringComparison.OrdinalIgnoreCase)), "Invalid key: Key is reserved for internal use: ""%s""", key)

			Try
					Using zipFile As New ZipFile(f)
					Dim entry As ZipEntry = zipFile.getEntry("objects/" & key)
					If entry Is Nothing Then
						Throw New System.InvalidOperationException("No object with key """ & key & """ found")
					End If
        
					Dim o As Object
					Using ois As New ObjectInputStream(New BufferedInputStream(zipFile.getInputStream(entry)))
						o = ois.readObject()
					End Using
					zipFile.close()
					Return DirectCast(o, T)
					End Using
			Catch e As Exception When TypeOf e Is IOException OrElse TypeOf e Is ClassNotFoundException
				Throw New Exception("Error reading object (key = " & key & ") from file " & f, e)
			End Try
		End Function

		''' <summary>
		''' List the keys of all objects added using the method <seealso cref="addObjectToFile(File, String, Object)"/> </summary>
		''' <param name="f"> File previously created with ModelSerializer </param>
		''' <returns> List of keys that can be used with <seealso cref="getObjectFromFile(File, String)"/> </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static java.util.List<String> listObjectsInFile(@NonNull File f)
		Public Shared Function listObjectsInFile(ByVal f As File) As IList(Of String)
			Preconditions.checkState(f.exists(), "File must exist: %s", f)

			Dim [out] As IList(Of String) = New List(Of String)()
			Try
					Using zipFile As New ZipFile(f)
        
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.util.Iterator<? extends java.util.zip.ZipEntry> entries = zipFile.entries();
					Dim entries As IEnumerator(Of ZipEntry) = zipFile.entries()
					Do While entries.MoveNext()
						Dim e As ZipEntry = entries.Current
						Dim name As String = e.getName()
						If Not e.isDirectory() AndAlso name.StartsWith("objects/", StringComparison.Ordinal) Then
							Dim s As String = name.Substring(8)
							[out].Add(s)
						End If
					Loop
					Return [out]
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function



		''' <summary>
		''' This method restores normalizer from a given persisted model file
		''' 
		''' PLEASE NOTE: File should be model file saved earlier with ModelSerializer with addNormalizerToModel being called
		''' </summary>
		''' <param name="file">
		''' @return </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.nd4j.linalg.dataset.api.preprocessor.Normalizer> T restoreNormalizerFromFile(File file) throws IOException
		Public Shared Function restoreNormalizerFromFile(Of T As Normalizer)(ByVal file As File) As T
			Try
					Using [is] As Stream = New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
					Return restoreNormalizerFromInputStream([is])
					End Using
			Catch e As Exception
				log.warn("Error while restoring normalizer, trying to restore assuming deprecated format...")
				Dim restoredDeprecated As DataNormalization = restoreNormalizerFromInputStreamDeprecated(New FileStream(file, FileMode.Open, FileAccess.Read))

				log.warn("Recovered using deprecated method. Will now re-save the normalizer to fix this issue.")
				addNormalizerToModel(file, restoredDeprecated)

				Return DirectCast(restoredDeprecated, T)
			End Try
		End Function


		''' <summary>
		''' This method restores the normalizer form a persisted model file.
		''' </summary>
		''' <param name="is"> A stream to load data from. </param>
		''' <returns> the loaded normalizer </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T extends org.nd4j.linalg.dataset.api.preprocessor.Normalizer> T restoreNormalizerFromInputStream(InputStream is) throws IOException
		Public Shared Function restoreNormalizerFromInputStream(Of T As Normalizer)(ByVal [is] As Stream) As T
			checkInputStream([is])
			Dim files As IDictionary(Of String, SByte()) = loadZipData([is])
			Return restoreNormalizerFromMap(files)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static <T extends org.nd4j.linalg.dataset.api.preprocessor.Normalizer> T restoreNormalizerFromMap(java.util.Map<String, byte[]> files) throws IOException
		Private Shared Function restoreNormalizerFromMap(Of T As Normalizer)(ByVal files As IDictionary(Of String, SByte())) As T
			Dim norm() As SByte = files(NORMALIZER_BIN)

			' checking for file existence
			If norm Is Nothing Then
				Return Nothing
			End If
			Try
				Return NormalizerSerializer.Default.restore(New MemoryStream(norm))
			Catch e As Exception
				Throw New IOException("Error loading normalizer", e)
			End Try
		End Function

		''' <summary>
		''' @deprecated
		''' 
		''' This method restores normalizer from a given persisted model file serialized with Java object serialization
		''' 
		''' </summary>
		Private Shared Function restoreNormalizerFromInputStreamDeprecated(ByVal stream As Stream) As DataNormalization
			Try
				Dim ois As New ObjectInputStream(stream)
				Try
					Dim normalizer As DataNormalization = DirectCast(ois.readObject(), DataNormalization)
					Return normalizer
				Catch e As ClassNotFoundException
					Throw New Exception(e)
				End Try
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void checkInputStream(InputStream inputStream) throws IOException
		Private Shared Sub checkInputStream(ByVal inputStream As Stream)
			'available method can return 0 in some cases: https://github.com/eclipse/deeplearning4j/issues/4887
			Dim available As Integer
			Try
				'InputStream.available(): A subclass' implementation of this method may choose to throw an IOException
				' if this input stream has been closed by invoking the close() method.
				available = inputStream.available()
			Catch e As IOException
				Throw New IOException("Cannot read from stream: stream may have been closed or is attempting to be read from" & "multiple times?", e)
			End Try
			If available <= 0 Then
				Throw New IOException("Cannot read from stream: stream may have been closed or is attempting to be read from" & "multiple times?")
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static java.util.Map<String, byte[]> loadZipData(InputStream is) throws IOException
		Private Shared Function loadZipData(ByVal [is] As Stream) As IDictionary(Of String, SByte())
			Dim result As IDictionary(Of String, SByte()) = New Dictionary(Of String, SByte())()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: try (final java.util.zip.ZipInputStream zis = new java.util.zip.ZipInputStream(is))
			Using zis As New java.util.zip.ZipInputStream([is])
				Do
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.zip.ZipEntry zipEntry = zis.getNextEntry();
					Dim zipEntry As ZipEntry = zis.getNextEntry()
					If zipEntry Is Nothing Then
						Exit Do
					End If
					If zipEntry.isDirectory() OrElse zipEntry.getSize() > Integer.MaxValue Then
						Throw New System.ArgumentException()
					End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int size = (int)(zipEntry.getSize());
					Dim size As Integer = CInt(Math.Truncate(zipEntry.getSize()))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final byte[] data;
					Dim data() As SByte
					If size >= 0 Then ' known size
						data = IOUtils.readFully(zis, size)
					Else ' unknown size
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ByteArrayOutputStream bout = new ByteArrayOutputStream();
						Dim bout As New MemoryStream()
						IOUtils.copy(zis, bout)
						data = bout.toByteArray()
					End If
									result(zipEntry.getName()) = data
				Loop
			End Using
			Return result
		End Function

	End Class

End Namespace