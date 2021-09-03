Imports System
Imports NonNull = lombok.NonNull
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports Hdf5Archive = org.deeplearning4j.nn.modelimport.keras.Hdf5Archive
Imports KerasModelConfiguration = org.deeplearning4j.nn.modelimport.keras.config.KerasModelConfiguration
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

Namespace org.deeplearning4j.nn.modelimport.keras.utils


	Public Class DL4JKerasModelValidator

		Private Sub New()
		End Sub

		''' <summary>
		''' Validate whether the file represents a valid Keras Sequential model (HDF5 archive)
		''' </summary>
		''' <param name="f"> File that should represent an saved Keras Sequential model (HDF5 archive) </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateKerasSequential(@NonNull File f)
		Public Shared Function validateKerasSequential(ByVal f As File) As ValidationResult
			Return validateKeras(f, "Keras Sequential Model HDF5", GetType(MultiLayerNetwork))
		End Function

		''' <summary>
		''' Validate whether the file represents a valid Keras Functional model (HDF5 archive)
		''' </summary>
		''' <param name="f"> File that should represent an saved Keras Functional model (HDF5 archive) </param>
		''' <returns> Result of validation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.common.validation.ValidationResult validateKerasFunctional(@NonNull File f)
		Public Shared Function validateKerasFunctional(ByVal f As File) As ValidationResult
			Return validateKeras(f, "Keras Functional Model HDF5", GetType(ComputationGraph))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected static org.nd4j.common.validation.ValidationResult validateKeras(@NonNull File f, String format, @Class cl)
		Protected Friend Shared Function validateKeras(ByVal f As File, ByVal format As String, ByVal cl As Type) As ValidationResult
			Dim vr As ValidationResult = Nd4jCommonValidator.isValidFile(f, format, False)
			If vr IsNot Nothing AndAlso Not vr.isValid() Then
				Return vr
			End If

			Dim c As New KerasModelConfiguration()
			Dim archive As Hdf5Archive = Nothing
			Try
				archive = New Hdf5Archive(f.getPath())

				'Check JSON
				Try
					Dim json As String = archive.readAttributeAsJson(c.getTrainingModelConfigAttribute())
					vr = Nd4jCommonValidator.isValidJSON(json)
					If vr IsNot Nothing AndAlso Not vr.isValid() Then
						vr.setFormatType(format)
						Return vr
					End If
				Catch t As Exception
					Return ValidationResult.builder().formatType(format).formatClass(cl).valid(False).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Unable to read JSON configuration from Keras Sequential model HDF5 file")).exception(t).build()
				End Try

			Catch t As Exception
				Return ValidationResult.builder().formatType(format).formatClass(cl).valid(False).path(Nd4jCommonValidator.getPath(f)).issues(Collections.singletonList("Unable to read from " & format & " file - file is corrupt or not a valid Keras HDF5 archive?")).exception(t).build()
			End Try


			Return ValidationResult.builder().formatType(format).formatClass(cl).valid(True).path(Nd4jCommonValidator.getPath(f)).build()
		End Function
	End Class

End Namespace