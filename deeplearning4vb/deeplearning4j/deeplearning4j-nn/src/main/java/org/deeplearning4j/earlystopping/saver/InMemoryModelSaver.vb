Imports System
Imports org.deeplearning4j.earlystopping
Imports Model = org.deeplearning4j.nn.api.Model

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

Namespace org.deeplearning4j.earlystopping.saver


	Public Class InMemoryModelSaver(Of T As org.deeplearning4j.nn.api.Model)
		Implements EarlyStoppingModelSaver(Of T)

'JAVA TO VB CONVERTER NOTE: The field bestModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private bestModel_Conflict As T
'JAVA TO VB CONVERTER NOTE: The field latestModel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		<NonSerialized>
		Private latestModel_Conflict As T

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void saveBestModel(T net, double score) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub saveBestModel(ByVal net As T, ByVal score As Double) Implements EarlyStoppingModelSaver(Of T).saveBestModel
			Try
				'Necessary because close is protected :S
				bestModel_Conflict = CType((net.GetType().getDeclaredMethod("clone")).invoke(net), T)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public void saveLatestModel(T net, double score) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub saveLatestModel(ByVal net As T, ByVal score As Double) Implements EarlyStoppingModelSaver(Of T).saveLatestModel
			Try
				'Necessary because close is protected :S
				latestModel_Conflict = CType((net.GetType().getDeclaredMethod("clone")).invoke(net), T)
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public T getBestModel() throws java.io.IOException
		Public Overridable ReadOnly Property BestModel As T Implements EarlyStoppingModelSaver(Of T).getBestModel
			Get
				Return bestModel_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public T getLatestModel() throws java.io.IOException
		Public Overridable ReadOnly Property LatestModel As T Implements EarlyStoppingModelSaver(Of T).getLatestModel
			Get
				Return latestModel_Conflict
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return "InMemoryModelSaver()"
		End Function
	End Class

End Namespace