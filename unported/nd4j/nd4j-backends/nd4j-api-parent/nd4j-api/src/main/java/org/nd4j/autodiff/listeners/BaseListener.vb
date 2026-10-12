Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.nd4j.autodiff.listeners

	Public MustInherit Class BaseListener
		Implements Listener

		Public MustOverride Function isActive(ByVal operation As Operation) As Boolean Implements Listener.isActive


		Public Overridable Function requiredVariables(ByVal sd As SameDiff) As ListenerVariables Implements Listener.requiredVariables
			Return ListenerVariables.empty()
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub epochStart(ByVal sd As SameDiff, ByVal at_Conflict As At) Implements Listener.epochStart
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function epochEnd(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long) As ListenerResponse Implements Listener.epochEnd
			Return ListenerResponse.CONTINUE
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function validationDone(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal validationTimeMillis As Long) As ListenerResponse Implements Listener.validationDone
			'No op
			Return ListenerResponse.CONTINUE
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub iterationStart(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal data As MultiDataSet, ByVal etlMs As Long) Implements Listener.iterationStart
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
'JAVA TO VB CONVERTER NOTE: The parameter loss was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub iterationDone(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal dataSet As MultiDataSet, ByVal loss_Conflict As Loss) Implements Listener.iterationDone
			'No op
		End Sub

		Public Overridable Sub operationStart(ByVal sd As SameDiff, ByVal op As Operation) Implements Listener.operationStart
			'No op
		End Sub

		Public Overridable Sub operationEnd(ByVal sd As SameDiff, ByVal op As Operation) Implements Listener.operationEnd
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub preOpExecution(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal op As SameDiffOp, ByVal opContext As OpContext) Implements Listener.preOpExecution
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub opExecution(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray) Implements Listener.opExecution
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub activationAvailable(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal varName As String, ByVal activation As INDArray) Implements Listener.activationAvailable
			'No op
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter at was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Sub preUpdate(ByVal sd As SameDiff, ByVal at_Conflict As At, ByVal v As Variable, ByVal update As INDArray) Implements Listener.preUpdate
			'No op
		End Sub
	End Class

End Namespace