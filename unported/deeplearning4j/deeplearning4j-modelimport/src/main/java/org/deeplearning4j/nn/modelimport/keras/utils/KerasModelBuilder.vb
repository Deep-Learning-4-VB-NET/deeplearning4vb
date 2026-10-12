Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Data = lombok.Data
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Hdf5Archive = org.deeplearning4j.nn.modelimport.keras.Hdf5Archive
Imports KerasLayer = org.deeplearning4j.nn.modelimport.keras.KerasLayer
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSequentialModel = org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel
Imports KerasModelConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasModelConfiguration
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

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

Namespace org.deeplearning4j.nn.modelimport.keras.utils


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class KerasModelBuilder implements Cloneable, Closeable
	Public Class KerasModelBuilder
		Implements Cloneable, System.IDisposable

'JAVA TO VB CONVERTER NOTE: The field modelJson was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend modelJson_Conflict As String = Nothing
'JAVA TO VB CONVERTER NOTE: The field modelYaml was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend modelYaml_Conflict As String = Nothing
'JAVA TO VB CONVERTER NOTE: The field trainingJson was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend trainingJson_Conflict As String = Nothing
'JAVA TO VB CONVERTER NOTE: The field trainingYaml was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend trainingYaml_Conflict As String = Nothing
		Protected Friend weightsArchive As Hdf5Archive = Nothing
		Protected Friend weightsRoot As String = Nothing
		Protected Friend trainingArchive As Hdf5Archive = Nothing
'JAVA TO VB CONVERTER NOTE: The field enforceTrainingConfig was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend enforceTrainingConfig_Conflict As Boolean = False
		Protected Friend config As KerasModelConfiguration
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend inputShape_Conflict() As Integer = Nothing
'JAVA TO VB CONVERTER NOTE: The field dimOrder was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend dimOrder_Conflict As KerasLayer.DimOrder = Nothing


		''' <summary>
		''' KerasModelBuilder constructed from a model configuration.
		''' </summary>
		''' <param name="config"> KerasModelConfiguration </param>
		Public Sub New(ByVal config As KerasModelConfiguration)
			Me.config = config
		End Sub

		''' <summary>
		''' Set model architecture from model JSON string.
		''' </summary>
		''' <param name="modelJson"> model as JSON string. </param>
		''' <returns> Model Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter modelJson was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelJson(ByVal modelJson_Conflict As String) As KerasModelBuilder
			Me.modelJson_Conflict = modelJson_Conflict
			Return Me
		End Function

		''' <summary>
		''' Set model architecture from model YAML string.
		''' </summary>
		''' <param name="modelYaml"> model as YAML string. </param>
		''' <returns> Model Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter modelYaml was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelYaml(ByVal modelYaml_Conflict As String) As KerasModelBuilder
			Me.modelYaml_Conflict = modelYaml_Conflict
			Return Me
		End Function

		''' <summary>
		''' Set model architecture from file name pointing to model JSON string.
		''' </summary>
		''' <param name="modelJsonFilename"> Name of file containing model JSON string </param>
		''' <returns> Model Builder </returns>
		''' <exception cref="IOException"> I/O Exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder modelJsonFilename(String modelJsonFilename) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter modelJsonFilename was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelJsonFilename(ByVal modelJsonFilename_Conflict As String) As KerasModelBuilder
			checkForExistence(modelJsonFilename_Conflict)
			Me.modelJson_Conflict = New String(Files.readAllBytes(Paths.get(modelJsonFilename_Conflict)))
			Return Me
		End Function

		''' <summary>
		''' Set model architecture from file name pointing to model YAML string.
		''' </summary>
		''' <param name="modelYamlFilename"> Name of file containing model YAML string </param>
		''' <returns> Model Builder </returns>
		''' <exception cref="IOException"> I/O Exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder modelYamlFilename(String modelYamlFilename) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter modelYamlFilename was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelYamlFilename(ByVal modelYamlFilename_Conflict As String) As KerasModelBuilder
			checkForExistence(modelYamlFilename_Conflict)
			Me.modelJson_Conflict = New String(Files.readAllBytes(Paths.get(modelYamlFilename_Conflict)))
			Return Me
		End Function

		''' <summary>
		''' Set model architecture from input stream of model JSON.
		''' </summary>
		''' <param name="modelJsonInputStream"> Input stream of model JSON </param>
		''' <returns> Model builder </returns>
		''' <exception cref="IOException"> I/O exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder modelJsonInputStream(InputStream modelJsonInputStream) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter modelJsonInputStream was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelJsonInputStream(ByVal modelJsonInputStream_Conflict As Stream) As KerasModelBuilder
			Dim byteArrayOutputStream As New MemoryStream()
			IOUtils.copy(modelJsonInputStream_Conflict, byteArrayOutputStream)
			Me.modelJson_Conflict = StringHelper.NewString(byteArrayOutputStream.toByteArray())
			Return Me
		End Function

		''' <summary>
		''' Set model architecture from input stream of model YAML.
		''' </summary>
		''' <param name="modelYamlInputStream"> Input stream of model YAML </param>
		''' <returns> Model builder </returns>
		''' <exception cref="IOException"> I/O exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder modelYamlInputStream(InputStream modelYamlInputStream) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter modelYamlInputStream was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelYamlInputStream(ByVal modelYamlInputStream_Conflict As Stream) As KerasModelBuilder
			Dim byteArrayOutputStream As New MemoryStream()
			IOUtils.copy(modelYamlInputStream_Conflict, byteArrayOutputStream)
			Me.modelJson_Conflict = StringHelper.NewString(byteArrayOutputStream.toByteArray())
			Return Me
		End Function

		''' <summary>
		''' Provide input shape for Keras models that have been compiled without one. DL4J
		''' needs this shape information on import to infer shapes of later layers and do
		''' shape validation.
		''' </summary>
		''' <param name="inputShape"> Input shape as int array </param>
		''' <returns> Model Builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter inputShape was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function inputShape(ByVal inputShape_Conflict() As Integer) As KerasModelBuilder
			Me.inputShape_Conflict = inputShape_Conflict
			Return Me
		End Function

		''' <summary>
		''' Provide training configuration as JSON string
		''' </summary>
		''' <param name="trainingJson"> Training JSON string </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter trainingJson was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function trainingJson(ByVal trainingJson_Conflict As String) As KerasModelBuilder
			Me.trainingJson_Conflict = trainingJson_Conflict
			Return Me
		End Function

		''' <summary>
		''' Provide training configuration as YAML string
		''' </summary>
		''' <param name="trainingYaml"> Training YAML string </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter trainingYaml was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function trainingYaml(ByVal trainingYaml_Conflict As String) As KerasModelBuilder
			Me.trainingYaml_Conflict = trainingYaml_Conflict
			Return Me
		End Function

		''' <summary>
		''' Manually set dim order for Keras model, i.e. either TENSORFLOW (channels last)
		''' or THEANO (channels first).
		''' 
		''' Dim ordering will be automatically inferred from your model file, so don't
		''' tamper with this option unless you're sure what you're doing. Explicitly
		''' setting dim ordering can be useful for very old Keras models (before version 1.2),
		''' for which inference can be difficult.
		''' </summary>
		''' <param name="dimOrder"> Ordering of dimensions (channels first vs. last) </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter dimOrder was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function dimOrder(ByVal dimOrder_Conflict As KerasLayer.DimOrder) As KerasModelBuilder
			Me.dimOrder_Conflict = dimOrder_Conflict
			Return Me
		End Function

		''' <summary>
		''' Provide training configuration as file input stream from JSON
		''' </summary>
		''' <param name="trainingJsonInputStream"> Input stream of training JSON string </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder trainingJsonInputStream(InputStream trainingJsonInputStream) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter trainingJsonInputStream was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function trainingJsonInputStream(ByVal trainingJsonInputStream_Conflict As Stream) As KerasModelBuilder
			Dim byteArrayOutputStream As New MemoryStream()
			IOUtils.copy(trainingJsonInputStream_Conflict, byteArrayOutputStream)
			Me.trainingJson_Conflict = StringHelper.NewString(byteArrayOutputStream.toByteArray())
			Return Me
		End Function

		''' <summary>
		''' Provide training configuration as file input stream from YAML
		''' </summary>
		''' <param name="trainingYamlInputStream"> Input stream of training YAML string </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder trainingYamlInputStream(InputStream trainingYamlInputStream) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter trainingYamlInputStream was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function trainingYamlInputStream(ByVal trainingYamlInputStream_Conflict As Stream) As KerasModelBuilder
			Dim byteArrayOutputStream As New MemoryStream()
			IOUtils.copy(trainingYamlInputStream_Conflict, byteArrayOutputStream)
			Me.trainingYaml_Conflict = StringHelper.NewString(byteArrayOutputStream.toByteArray())
			Return Me
		End Function

		''' <summary>
		''' Set full model HDF5 (architecture, weights and training configuration) by providing the HDF5 filename.
		''' </summary>
		''' <param name="modelHdf5Filename"> File name of HDF5 file containing full model </param>
		''' <returns> Model builder </returns>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported configuration </exception>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid configuration </exception>
		''' <exception cref="IOException"> I/O exception </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder modelHdf5Filename(String modelHdf5Filename) throws UnsupportedKerasConfigurationException, InvalidKerasConfigurationException, IOException
'JAVA TO VB CONVERTER NOTE: The parameter modelHdf5Filename was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function modelHdf5Filename(ByVal modelHdf5Filename_Conflict As String) As KerasModelBuilder
			checkForExistence(modelHdf5Filename_Conflict)
			SyncLock Hdf5Archive.LOCK_OBJECT
				Try
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: this.weightsArchive = this.trainingArchive = new org.deeplearning4j.nn.modelimport.keras.Hdf5Archive(modelHdf5Filename);
					Me.trainingArchive = New Hdf5Archive(modelHdf5Filename_Conflict)
						Me.weightsArchive = Me.trainingArchive
					Me.weightsRoot = config.getTrainingWeightsRoot()
					If Not Me.weightsArchive.hasAttribute(config.getTrainingModelConfigAttribute()) Then
						Throw New InvalidKerasConfigurationException("Model configuration attribute missing from " & modelHdf5Filename_Conflict & " archive.")
					End If
					Dim initialModelJson As String = Me.weightsArchive.readAttributeAsJson(config.getTrainingModelConfigAttribute())

					Dim kerasVersion As String = Me.weightsArchive.readAttributeAsFixedLengthString(config.getFieldKerasVersion(), 5)
					Dim modelMapper As IDictionary(Of String, Object) = KerasModelUtils.parseJsonString(initialModelJson)
					modelMapper(config.getFieldKerasVersion()) = kerasVersion

					Dim majorKerasVersion As Integer = CInt(Math.Truncate(Char.GetNumericValue(kerasVersion.Chars(0))))
					If majorKerasVersion = 2 Then
						Dim backend As String = Me.weightsArchive.readAttributeAsString(config.getFieldBackend())
						modelMapper(config.getFieldBackend()) = backend
					End If

					Me.modelJson_Conflict = (New ObjectMapper()).writeValueAsString(modelMapper)
					If Me.trainingArchive.hasAttribute(config.getTrainingTrainingConfigAttribute()) Then
						Me.trainingJson_Conflict = Me.trainingArchive.readAttributeAsJson(config.getTrainingTrainingConfigAttribute())
					End If
				Catch t As Exception
					Dispose()
					Throw t
				End Try
			End SyncLock
			Return Me
		End Function

		''' <summary>
		''' Set weights of the model by providing the file name of the corresponding weights HDF5 file.
		''' The root of the HDF5 group containing weights won't be set by this method.
		''' </summary>
		''' <param name="weightsHdf5Filename"> File name of weights HDF5 </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder weightsHdf5FilenameNoRoot(String weightsHdf5Filename) throws IOException
		Public Overridable Function weightsHdf5FilenameNoRoot(ByVal weightsHdf5Filename As String) As KerasModelBuilder
			checkForExistence(weightsHdf5Filename)
			Me.weightsArchive = New Hdf5Archive(weightsHdf5Filename)
			Return Me
		End Function

		''' <summary>
		''' Set weights of the model by providing the file name of the corresponding weights HDF5 file.
		''' The root of the HDF5 group containing weights will be read and set from the configuration of this
		''' model builder instance.
		''' </summary>
		''' <param name="weightsHdf5Filename"> File name of weights HDF5 </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public KerasModelBuilder weightsHdf5Filename(String weightsHdf5Filename) throws IOException
'JAVA TO VB CONVERTER NOTE: The parameter weightsHdf5Filename was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function weightsHdf5Filename(ByVal weightsHdf5Filename_Conflict As String) As KerasModelBuilder
			checkForExistence(weightsHdf5Filename_Conflict)
			Me.weightsArchive = New Hdf5Archive(weightsHdf5Filename_Conflict)
			Me.weightsRoot = config.getTrainingWeightsRoot()
			Return Me
		End Function

		''' <summary>
		''' Determine whether to enforce loading a training configuration or not.
		''' </summary>
		''' <param name="enforceTrainingConfig"> boolean, read training config or not </param>
		''' <returns> Model builder </returns>
'JAVA TO VB CONVERTER NOTE: The parameter enforceTrainingConfig was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function enforceTrainingConfig(ByVal enforceTrainingConfig_Conflict As Boolean) As KerasModelBuilder
			Me.enforceTrainingConfig_Conflict = enforceTrainingConfig_Conflict
			Return Me
		End Function

		''' <summary>
		''' Build a KerasModel (corresponding to ComputationGraph) from this model builder.
		''' </summary>
		''' <returns> KerasModel </returns>
		''' <exception cref="IOException"> I/O exception </exception>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.modelimport.keras.KerasModel buildModel() throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function buildModel() As KerasModel
			Dim model As New KerasModel(Me)
			Dispose()
			Return model
		End Function

		''' <summary>
		''' Build a KerasSequentialModel (corresponding to MultiLayerNetwork) from this model builder.
		''' </summary>
		''' <returns> KerasSequentialModel </returns>
		''' <exception cref="IOException"> I/O exception </exception>
		''' <exception cref="InvalidKerasConfigurationException"> Invalid configuration </exception>
		''' <exception cref="UnsupportedKerasConfigurationException"> Unsupported configuration </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel buildSequential() throws IOException, InvalidKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
		Public Overridable Function buildSequential() As KerasSequentialModel
			Dim sequentialModel As New KerasSequentialModel(Me)
			Dispose()
			Return sequentialModel
		End Function

		''' <summary>
		''' Close all HDF5 archives for this model builder.
		''' </summary>
		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			If trainingArchive IsNot Nothing AndAlso trainingArchive IsNot weightsArchive Then
				trainingArchive.Dispose()
				trainingArchive = Nothing
			End If
			If weightsArchive IsNot Nothing Then
				weightsArchive.Dispose()
				weightsArchive = Nothing
			End If
		End Sub

		''' <summary>
		''' Check if the file corresponding to model JSON/YAML or HDF5 files actually exists
		''' and throw an explicit exception.
		''' </summary>
		''' <param name="fileName"> File name to check for existence </param>
		''' <exception cref="FileNotFoundException"> File not found </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void checkForExistence(String fileName) throws IOException
		Private Sub checkForExistence(ByVal fileName As String)
			Dim file As New File(fileName)
			If Not file.exists() Then
				Throw New FileNotFoundException("File with name " & fileName & " does not exist.")
			End If
			If Not file.isFile() Then
				Throw New IOException("Provided string does not correspond to an actual file.")
			End If

		End Sub
	End Class
End Namespace