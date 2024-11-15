﻿Imports System
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Environment = org.nd4j.linalg.factory.Environment
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertFalse

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
Namespace org.nd4j.linalg.api

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class TestEnvironment extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestEnvironment
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEnvironment(org.nd4j.linalg.factory.Nd4jBackend backend)
		  Public Overridable Sub testEnvironment(ByVal backend As Nd4jBackend)
			Dim e As Environment = Nd4j.Environment
			Console.WriteLine("BLAS version: " & e.blasMajorVersion() & "." & e.blasMinorVersion() & "." & e.blasPatchVersion())
			Console.WriteLine("CPU: " & e.CPU)
			Console.WriteLine("Helpers allowed: " & e.helpersAllowed())
			assertFalse(e.Verbose)
			assertFalse(e.Debug)
			assertFalse(e.DebugAndVerbose)
			Console.WriteLine("Max master threads: " & e.maxMasterThreads())
		  End Sub
	End Class

End Namespace