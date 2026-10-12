Imports System
Imports System.Collections.Generic
Imports HashMultiset = org.nd4j.shade.guava.collect.HashMultiset
Imports Multiset = org.nd4j.shade.guava.collect.Multiset
Imports Getter = lombok.Getter

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

Namespace org.deeplearning4j.eval


'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: @Deprecated public class ConfusionMatrix<T extends Comparable<? super T>> extends org.nd4j.evaluation.classification.ConfusionMatrix<T>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
	<Obsolete>
	Public Class ConfusionMatrix(Of T As IComparable(Of Object))
		Inherits org.nd4j.evaluation.classification.ConfusionMatrix(Of T)

		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ConfusionMatrix""/>")>
		Public Sub New(ByVal classes As IList(Of T))
			MyBase.New(classes)
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ConfusionMatrix"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ConfusionMatrix""/>")>
		Public Sub New()
			MyBase.New()
		End Sub

		''' @deprecated Use <seealso cref="org.nd4j.evaluation.classification.ConfusionMatrix"/> 
		<Obsolete("Use <seealso cref=""org.nd4j.evaluation.classification.ConfusionMatrix""/>")>
		Public Sub New(ByVal other As ConfusionMatrix(Of T))
			MyBase.New(other)
		End Sub
	End Class

End Namespace