Imports System.Threading
Imports StackDescriptor = org.nd4j.linalg.profiler.data.primitives.StackDescriptor
Imports StackTree = org.nd4j.linalg.profiler.data.primitives.StackTree

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

Namespace org.nd4j.linalg.profiler.data

	Public Class StackAggregator
		Private tree As New StackTree()

		Public Sub New()
			' nothing to do here so far
		End Sub

		Public Overridable Sub renderTree()
			tree.renderTree(False)
		End Sub

		Public Overridable Sub renderTree(ByVal displayCounts As Boolean)
			tree.renderTree(displayCounts)
		End Sub

		Public Overridable Sub reset()
			tree.reset()
		End Sub

		Public Overridable Sub incrementCount()
			incrementCount(1)
		End Sub

		Public Overridable Sub incrementCount(ByVal time As Long)
			Dim descriptor As New StackDescriptor(Thread.CurrentThread.getStackTrace())
			tree.consumeStackTrace(descriptor, time)
		End Sub

		Public Overridable ReadOnly Property TotalEventsNumber As Long
			Get
				Return tree.TotalEventsNumber
			End Get
		End Property

		Public Overridable ReadOnly Property UniqueBranchesNumber As Integer
			Get
				Return tree.UniqueBranchesNumber
			End Get
		End Property

		Public Overridable ReadOnly Property LastDescriptor As StackDescriptor
			Get
				Return tree.getLastDescriptor()
			End Get
		End Property
	End Class

End Namespace