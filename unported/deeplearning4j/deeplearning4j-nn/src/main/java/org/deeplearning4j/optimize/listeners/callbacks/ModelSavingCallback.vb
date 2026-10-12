Imports System
Imports NonNull = lombok.NonNull
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports Model = org.deeplearning4j.nn.api.Model
Imports EvaluativeListener = org.deeplearning4j.optimize.listeners.EvaluativeListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports org.nd4j.evaluation

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

Namespace org.deeplearning4j.optimize.listeners.callbacks


	Public Class ModelSavingCallback
		Implements EvaluationCallback

		Protected Friend rootFolder As File
		Protected Friend template As String

		''' <summary>
		''' This constructor will create ModelSavingCallback instance that will save models in current folder
		''' 
		''' PLEASE NOTE: Make sure you have write access to the current folder
		''' </summary>
		''' <param name="fileNameTemplate"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ModelSavingCallback(@NonNull String fileNameTemplate)
		Public Sub New(ByVal fileNameTemplate As String)
			Me.New(New File("./"), fileNameTemplate)
		End Sub

		''' <summary>
		''' This constructor will create ModelSavingCallback instance that will save models in specified folder
		''' 
		''' PLEASE NOTE: Make sure you have write access to the target folder
		''' </summary>
		''' <param name="rootFolder"> File object referring to target folder </param>
		''' <param name="fileNameTemplate"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ModelSavingCallback(@NonNull File rootFolder, @NonNull String fileNameTemplate)
		Public Sub New(ByVal rootFolder As File, ByVal fileNameTemplate As String)
			If Not rootFolder.isDirectory() Then
				Throw New DL4JInvalidConfigException("rootFolder argument should point to valid folder")
			End If

			If fileNameTemplate.Length = 0 Then
				Throw New DL4JInvalidConfigException("Filename template can't be empty String")
			End If

			Me.rootFolder = rootFolder
			Me.template = fileNameTemplate
		End Sub

		Public Overridable Sub [call](ByVal listener As EvaluativeListener, ByVal model As Model, ByVal invocationsCount As Long, ByVal evaluations() As IEvaluation) Implements EvaluationCallback.call

			Dim temp As String = template.replaceAll("%d", "" & invocationsCount)

			Dim finalName As String = FilenameUtils.concat(rootFolder.getAbsolutePath(), temp)
			save(model, finalName)
		End Sub


		''' <summary>
		''' This method saves model
		''' </summary>
		''' <param name="model"> </param>
		''' <param name="filename"> </param>
		Protected Friend Overridable Sub save(ByVal model As Model, ByVal filename As String)
			Try
				ModelSerializer.writeModel(model, filename, True)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Sub
	End Class

End Namespace