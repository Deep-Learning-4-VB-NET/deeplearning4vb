Imports System
Imports OpExecutioner = DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner.OpExecutioner
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.blas.impl

    ''' <summary>
    ''' The base class of the BLAS level implementations (Java <c>BaseLevel</c>).
    ''' <para>
    ''' Difference from Java: the check every routine makes before reporting a BLAS call to the op profiler
    ''' (<c>Nd4j.getExecutioner().getProfilingMode() == ProfilingMode.ALL</c>) is the overridable
    ''' <see cref="profilingModeAll"/>.
    ''' </para>
    ''' </summary>
    Public MustInherit Class BaseLevel

        ''' <summary>
        ''' True when the executioner's (deprecated) profiling mode is <c>ALL</c>: the BLAS routines then report their
        ''' operands to <c>OpProfiler.processBlasCall</c>. Reads <c>Nd4vb.Executioner</c>.
        ''' </summary>
        Protected Friend Overridable Function profilingModeAll() As Boolean
            Return Nd4vbT.Executioner.getProfilingMode() = OpExecutioner.ProfilingMode.ALL
        End Function

    End Class

End Namespace
