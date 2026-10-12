Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Tag = org.junit.jupiter.api.Tag
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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

Namespace org.deeplearning4j.spark.text

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestFunction implements org.apache.spark.api.java.function.@Function<Integer, Integer>
	Public Class TestFunction
		Implements [Function](Of Integer, Integer)

		Public Sub New(ByVal lst As IList(Of Integer))
			Me.lst = lst
		End Sub

		Public Overridable ReadOnly Property Lst As IList(Of Integer)
			Get
				Return lst
			End Get
		End Property

		Public Overridable ReadOnly Property A As Integer
			Get
				Return a
			End Get
		End Property

		Private lst As IList(Of Integer)
		Private a As Integer


		Public Overrides Function [call](ByVal i As Integer?) As Integer?
			lst.Add(i)
			a = 1000
			Return i.Value + 1
		End Function
	End Class


End Namespace