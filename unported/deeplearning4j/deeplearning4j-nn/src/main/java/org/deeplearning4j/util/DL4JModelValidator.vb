Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports NonNull = lombok.NonNull
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Nd4jCommonValidator = org.nd4j.common.validation.Nd4jCommonValidator
Imports ValidationResult = org.nd4j.common.validation.ValidationResult

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


	Public Class DL4JModelValidator

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate whether the file represents a valid MultiLayerNetwork saved previously with <seealso cref="MultiLayerNetwork.save(File)"/>
		''' or <seealso cref="ModelSerializer.writeModel(Model, File, Boolean)"/>, to be read with <seealso cref="MultiLayerNetwork.load(File, Boolean)"/>
		''' </summary>
		''' <param name="f"> File that should represent an saved MultiLayerNetwork </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateMultiLayerNetwork(@NonNull File f)
		Public Shared Function validateMultiLayerNetwork(ByVal f As File) As ValidationResult

			Dim requiredEntries As IList(Of String) = New List(Of String) From {ModelSerializer.CONFIGURATION_JSON, ModelSerializer.COEFFICIENTS_BIN} 'TODO no-params models... might be OK to have no params, but basically useless in practice

			Dim vr As ValidationResult = Nd4jCommonValidator.isValidZipFile(f, False, requiredEntries)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				vr.setFormatClass(GetType(MultiLayerNetwork))
				vr.setFormatType("MultiLayerNetwork")
				Return vr
			End If

			'Check that configuration (JSON) can actually be deserialized correctly...
			Dim config As String
			Try
					Using zf As New ZipFile(f)
					Dim ze As ZipEntry = zf.getEntry(ModelSerializer.CONFIGURATION_JSON)
					config = IOUtils.toString(New StreamReader(zf.getInputStream(ze), Encoding.UTF8))
					End Using
			Catch e As IOException
				Return ValidationResult.builder().formatType("MultiLayerNetwork").formatClass(GetType(MultiLayerNetwork)).valid(False).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Unable to read configuration from model zip file")).exception(e).build()
			End Try

			Try
				MultiLayerConfiguration.fromJson(config)
			Catch t As Exception
				Return ValidationResult.builder().formatType("MultiLayerNetwork").formatClass(GetType(MultiLayerNetwork)).valid(False).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Zip file JSON model configuration does not appear to represent a valid MultiLayerConfiguration")).exception(t).build()
			End Try

			'TODO should we check params too?

			Return ValidationResult.builder().formatType("MultiLayerNetwork").formatClass(GetType(MultiLayerNetwork)).valid(True).path(Nd4jCommonValidator.getPath(f)).build()
		End Function

		''' <summary>
		''' Validate whether the file represents a valid ComputationGraph saved previously with <seealso cref="ComputationGraph.save(File)"/>
		''' or <seealso cref="ModelSerializer.writeModel(Model, File, Boolean)"/>, to be read with <seealso cref="ComputationGraph.load(File, Boolean)"/>
		''' </summary>
		''' <param name="f"> File that should represent an saved MultiLayerNetwork </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateComputationGraph(@NonNull File f)
		Public Shared Function validateComputationGraph(ByVal f As File) As ValidationResult

			Dim requiredEntries As IList(Of String) = New List(Of String) From {ModelSerializer.CONFIGURATION_JSON, ModelSerializer.COEFFICIENTS_BIN} 'TODO no-params models... might be OK to have no params, but basically useless in practice

			Dim vr As ValidationResult = Nd4jCommonValidator.isValidZipFile(f, False, requiredEntries)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				vr.setFormatClass(GetType(ComputationGraph))
				vr.setFormatType("ComputationGraph")
				Return vr
			End If

			'Check that configuration (JSON) can actually be deserialized correctly...
			Dim config As String
			Try
					Using zf As New ZipFile(f)
					Dim ze As ZipEntry = zf.getEntry(ModelSerializer.CONFIGURATION_JSON)
					config = IOUtils.toString(New StreamReader(zf.getInputStream(ze), Encoding.UTF8))
					End Using
			Catch e As IOException
				Return ValidationResult.builder().formatType("ComputationGraph").formatClass(GetType(ComputationGraph)).valid(False).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Unable to read configuration from model zip file")).exception(e).build()
			End Try

			Try
				ComputationGraphConfiguration.fromJson(config)
			Catch t As Exception
				Return ValidationResult.builder().formatType("ComputationGraph").formatClass(GetType(ComputationGraph)).valid(False).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Zip file JSON model configuration does not appear to represent a valid ComputationGraphConfiguration")).exception(t).build()
			End Try

			'TODO should we check params too? (a) that it can be read, and (b) that it matches config (number of parameters, etc)

			Return ValidationResult.builder().formatType("ComputationGraph").formatClass(GetType(ComputationGraph)).valid(True).path(Nd4jCommonValidator.getPath(f)).build()
		End Function
	End Class

End Namespace