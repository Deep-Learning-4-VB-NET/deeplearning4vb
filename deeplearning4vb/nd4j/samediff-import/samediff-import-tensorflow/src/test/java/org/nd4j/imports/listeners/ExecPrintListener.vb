Imports System
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
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

Namespace org.nd4j.imports.listeners

	Public Class ExecPrintListener
		Inherits BaseListener

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return True
		End Function

		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Console.WriteLine("------ Op: " & op.Name & " - opName = " & op.Op.opName() & ", class = " & op.Op.GetType().FullName & " ------")
			For Each arr As INDArray In outputs
				Console.WriteLine(arr)
			Next arr
		End Sub
	End Class

End Namespace